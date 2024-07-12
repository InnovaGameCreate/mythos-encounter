using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UniRx;

public class VolumeUI : AdjustVolume
{
    private float _maxVol = 20;
    private float _minVol = -80;
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider seVolumeSlider;
    [SerializeField] private Slider footstepVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    [SerializeField] private float changeVolumeValue;

    // Start is called before the first frame update
    void Start()
    {
        masterVolumeSlider.value = Map(ConfigVolume.masterVolume, _minVol, _maxVol, 0, 1);
        seVolumeSlider.value = Map(ConfigVolume.seVolume, _minVol, _maxVol, 0, 1);
        footstepVolumeSlider.value = Map(ConfigVolume.footstepVolume, _minVol, _maxVol, 0, 1);
        bgmVolumeSlider.value = Map(ConfigVolume.bgmVolume, _minVol, _maxVol, 0, 1);
        OnChangeMasterVolume.Subscribe(_vol =>
        {
            masterVolumeSlider.value = Map(_vol, _minVol, _maxVol, 0, 1);
        }).AddTo(this);
        
        OnChangeSEVolume.Subscribe(_vol =>
        {
            seVolumeSlider.value = Map(_vol, _minVol, _maxVol, 0, 1);
        }).AddTo(this);

        OnChangeFootstepVolume.Subscribe(_vol =>
        {
            footstepVolumeSlider.value = Map(_vol, _minVol, _maxVol, 0, 1);
        }).AddTo(this);

        OnChangeBGMVolume.Subscribe(_vol =>
        {
           bgmVolumeSlider.value = Map(_vol, _minVol, _maxVol, 0, 1);
        }).AddTo(this);
    }

    private float Map(float _val, float _start1, float _step1, float _start2, float _stop2)
    {
        return _start2 + (_stop2 - _start2) * ((_val - _start1) / (_step1 - _start1));
    }
    public void MasterVolumeUp()
    {
        SetMasterVolume(ConfigVolume.masterVolume + changeVolumeValue, _mixer);
    }
    public void SEVolumeUp()
    {
        SetSEVolume(ConfigVolume.seVolume + changeVolumeValue, _mixer);
    }
    public void FootstepVolumeUp()
    {
        SetFootstepVolume(ConfigVolume.footstepVolume + changeVolumeValue, _mixer);
    }
    public void BGMVolumeUp()
    {
        SetBGMVolume(ConfigVolume.bgmVolume + changeVolumeValue, _mixer);
    }
    public void MasterVolumeDown()
    {
        SetMasterVolume(ConfigVolume.masterVolume - changeVolumeValue, _mixer);
    }
    public void SEVolumeDown()
    {
        SetSEVolume(ConfigVolume.seVolume - changeVolumeValue, _mixer);
    }
    public void FootstepVolumeDown()
    {
        SetFootstepVolume(ConfigVolume.footstepVolume - changeVolumeValue, _mixer);
    }
    public void BGMVolumeDown()
    {
        SetBGMVolume(ConfigVolume.bgmVolume - changeVolumeValue, _mixer);
    }
}
