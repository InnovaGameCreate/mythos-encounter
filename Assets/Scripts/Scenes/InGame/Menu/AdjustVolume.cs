using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;
using System;

public class AdjustVolume : MonoBehaviour
{
    private float _maxVolume = 20;
    private float _minVolume = -80;

    private Subject<float> _changeMasterVolume = new Subject<float>();
    private Subject<float> _changeSEVolume = new Subject<float>();
    private Subject<float> _changeFootstepVolume = new Subject<float>();
    private Subject<float> _changeBGMVolume = new Subject<float>();

    public IObservable<float> OnChangeMasterVolume => _changeMasterVolume;
    public IObservable<float> OnChangeSEVolume => _changeSEVolume;
    public IObservable<float> OnChangeFootstepVolume => _changeFootstepVolume;
    public IObservable<float> OnChangeBGMVolume => _changeBGMVolume;


    public void SetMasterVolume(float _setVol, AudioMixer _mixer)
    {        
        if(_setVol > _maxVolume) { ConfigVolume.masterVolume = _maxVolume; }
        else if(_setVol < _minVolume) { ConfigVolume.masterVolume = _minVolume; }
        else { ConfigVolume.masterVolume = _setVol; }

        _mixer.SetFloat("Master", ConfigVolume.masterVolume);
        _changeMasterVolume.OnNext(ConfigVolume.masterVolume);
    }
    public void SetSEVolume(float _setVol, AudioMixer _mixer)
    {
        if (_setVol > _maxVolume) { ConfigVolume.seVolume = _maxVolume; }
        else if (_setVol < _minVolume) { ConfigVolume.seVolume = _minVolume; }
        else { ConfigVolume.seVolume = _setVol; }

        _mixer.SetFloat("SE", ConfigVolume.seVolume);
        _changeSEVolume.OnNext(ConfigVolume.seVolume);
    }
    public void SetFootstepVolume(float _setVol, AudioMixer _mixer)
    {
        if (_setVol > _maxVolume) { ConfigVolume.footstepVolume = _maxVolume; }
        else if (_setVol < _minVolume) { ConfigVolume.footstepVolume = _minVolume; }
        else { ConfigVolume.footstepVolume = _setVol; }

        _mixer.SetFloat("FootstepSE", ConfigVolume.footstepVolume);
        _changeFootstepVolume.OnNext(ConfigVolume.footstepVolume);
    }
    public void SetBGMVolume(float _setVol, AudioMixer _mixer)
    {
        if (_setVol > _maxVolume) { ConfigVolume.bgmVolume = _maxVolume; }
        else if (_setVol < _minVolume) { ConfigVolume.bgmVolume = _minVolume; }
        else { ConfigVolume.bgmVolume = _setVol; }

        _mixer.SetFloat("BGM", ConfigVolume.bgmVolume);
        _changeBGMVolume.OnNext(ConfigVolume.bgmVolume);
    }
}
