using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class CinematicManager : MonoBehaviour
{
    [SerializeField] Volume _blinkingVolume;
    [SerializeField] float target1;
    [SerializeField] float target2;
    [SerializeField] float target3;
    [SerializeField] Image Panel;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] private AudioClip _dialogClip;
    /*[SerializeField] float time1;
    [SerializeField] float time2;
    [SerializeField] float time3;
    float _speed1;
    float _speed2;
    float _speed3;*/
    [SerializeField, ReadOnly] private float _intensityTarget;
    private float _speed;
    private bool _panelEnabled;
    private LocalizeStringEvent _localizeStringEvent;

    private void Start()
    {
        StartCoroutine(PlayCinematic());
        _localizeStringEvent = Text.GetComponent<LocalizeStringEvent>();
    }

    private IEnumerator PlayCinematic()
    {
        _intensityTarget = 0f;
        _speed = 0.2f;
        AudioSystem.Instance.PlaySound(_dialogClip, Vector3.zero);
        yield return new WaitForSeconds(3f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog2";
        yield return new WaitForSeconds(3f);
        _intensityTarget = target1;
        yield return new WaitForSeconds(1f);
        _intensityTarget = 0f;
        yield return new WaitForSeconds(1f);
        _localizeStringEvent.StringReference.TableEntryReference = "Dialog3";
        yield return new WaitForSeconds(3f);
        _intensityTarget = target2;
        yield return new WaitForSeconds(3f);
        _intensityTarget = 0f;
        yield return new WaitForSeconds(2f);
        _intensityTarget = target3;
        yield return new WaitForSeconds(2f);
        _panelEnabled = true;
    }

    private void Update()
    {
        _blinkingVolume.weight = Mathf.MoveTowards(_blinkingVolume.weight, _intensityTarget, _speed * Time.deltaTime);
        if (_panelEnabled)
        {
            Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, Mathf.MoveTowards(Panel.color.a, 1f, 0.2f * Time.deltaTime));
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Mathf.MoveTowards(Text.color.a, 0f, 0.2f * Time.deltaTime));
            if (Panel.color.a == 1f)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            }
        }
    }

}
