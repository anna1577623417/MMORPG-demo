using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystemConfig : UIWindow {

	[SerializeField] Image musicOff;
	[SerializeField] Image soundOff;

	[SerializeField] Toggle toggleMusic;
    [SerializeField] Toggle toggleSound;

	[SerializeField] Slider sliderMusic;
	[SerializeField] Slider sliderSound;

    void Start () {
		this.toggleMusic.isOn = Config.MusicOn; musicOff.enabled = !Config.MusicOn;

        this.toggleSound.isOn = Config.SoundOn; soundOff.enabled = !Config.SoundOn;

        this.sliderMusic.value = Config.MusicVolume;
		this.sliderSound.value = Config.SoundVolume;

	}
    public override void OnyesClick() {
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		PlayerPrefs.Save();
		base.OnyesClick();
    }

	public void MusicToggle(bool on) {
		musicOff.enabled = !on;
		Config.MusicOn = on;
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	}
	public void SoundToggle(bool on) {
		soundOff.enabled = !on;
		Config.SoundOn = on;
		SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
	}

	public void MusicVolume(float volume) {
		Config.MusicVolume = (int)volume;
		PlaySound();
	}

	public void SoundVolume(float volume) {
		Config.SoundVolume = (int)volume;
		PlaySound();//试音，反馈音量大小
    }

	float lastPlay = 0;
	private void PlaySound() {
		if(Time.realtimeSinceStartup - lastPlay > 0.1) {
			lastPlay = Time.realtimeSinceStartup;
			SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
		}
	}

    public override void OnCloseClick() {
		base.OnCloseClick();
        //PlayerPrefs.Save();
    }
}
