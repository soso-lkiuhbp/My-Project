using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AccountMono : MonoBehaviour
{
    RectTransform _loginTip;
    RectTransform _registerTip;
    TMP_Text _loginTipText;
    TMP_Text _registerTipText;

    bool _showLoginTip;
    bool _showRegisterTip;
    float _loginTipWorkTime;
    float _registerTipWorkTime;
    const float TipShowDuring = 2f;

    Vector2 _originLoginTipPos;
    Vector2 _originRegisterTipPos;
    Vector2 _desLoginTipPos;
    Vector2 _desRegisterTipPos;
    bool _isDown;
    Button _sendBtn;
    TMP_Text _sendTip;
    float _sendWorkTime;
    const float SendDuring = 60;

    // Start is called before the first frame update
    void Start()
    {
        _loginTip = transform.Find("Login/Tip") as RectTransform;
        _registerTip = transform.Find("Register/Tip") as RectTransform;
        _loginTipText = _loginTip.GetComponentInChildren<TMP_Text>();
        _registerTipText = _registerTip . GetComponentInChildren<TMP_Text>();
        _originLoginTipPos = _loginTip.anchoredPosition;
        _originRegisterTipPos = _registerTip.anchoredPosition;
        _desLoginTipPos = _originLoginTipPos + Vector2.down * 165;
        _desRegisterTipPos = _originRegisterTipPos + Vector2.down * 165;
        _sendBtn = transform.Find("Register/SendBtn").GetComponent<Button>();
        _sendTip = _sendBtn.GetComponentInChildren<TMP_Text>();
    }

    public void SendCode()
    {
        _sendBtn.interactable = false;
        _sendWorkTime = 0;
    }

    public void ShowLoginTip(string content)
    {
        _loginTipText.text = content;
        _showLoginTip = true;
        _loginTipWorkTime = 0;
        _isDown = true;
    }

    public void ShowRegisterTip(string content)
    {
        _registerTipText.text = content;
        _showRegisterTip = true;
        _registerTipWorkTime = 0;
        _isDown = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(_showLoginTip)
        {
            _loginTipWorkTime += Time.deltaTime;
            if(_isDown)
            {
                _loginTip.anchoredPosition = Vector2.Lerp(_originLoginTipPos, _desLoginTipPos, _loginTipWorkTime);
            }
            else
            {
                _loginTip.anchoredPosition = Vector2.Lerp(_desLoginTipPos, _originLoginTipPos, _loginTipWorkTime);
            }

            if (_loginTipWorkTime >= TipShowDuring)
            {
                _loginTipWorkTime = 0;
                if(!_isDown)
                {
                    _showLoginTip = false;
                }
                _isDown = false;
            }
        }

        if (_showRegisterTip)
        {
            _registerTipWorkTime += Time.deltaTime;
            if (_isDown)
            {
                _registerTip.anchoredPosition = Vector2.Lerp(_originRegisterTipPos, _desRegisterTipPos, _registerTipWorkTime);
            }
            else
            {
                _registerTip.anchoredPosition = Vector2.Lerp(_desRegisterTipPos, _originRegisterTipPos, _registerTipWorkTime);
            }

            if (_registerTipWorkTime >= TipShowDuring)
            {
                _registerTipWorkTime = 0;
                if (!_isDown)
                {
                    _showRegisterTip = false;
                }
                _isDown = false;
            }
        }
        if(!_sendBtn.interactable)
        {
            _sendWorkTime += Time.deltaTime;
            _sendTip.text = "已发送(" + (SendDuring - _sendWorkTime).ToString("f0") + ")";
            if(_sendWorkTime >= SendDuring)
            {
                _sendBtn.interactable = true;
            }
        }
    }
}
