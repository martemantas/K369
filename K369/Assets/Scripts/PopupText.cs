using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopupText : MonoBehaviour
{
     [SerializeField] float _startingVelocity = 750f;
     [SerializeField] float _velocityDecayRate =  1500f;
     [SerializeField] float _timeBeforeFadeStarts = 0.6f;
     [SerializeField] float _fadeSpeed = 3f;
     
     TextMeshProUGUI _clickAmountText;
     
     Vector2 _currentVelocity;

     Color _startColor;
     float _timer;
     float _textAlpha;

     private void OnEnable()
     {
          _clickAmountText = GetComponent<TextMeshProUGUI>();

          Color newColor = _clickAmountText.color;
          newColor.a = 1f;
          _clickAmountText.color = newColor;

          _startColor = newColor;
          _timer = 0f;
          _textAlpha = 1f;
     }
     

     public void Setup(double amount)
     {
          _clickAmountText.text = "+" + amount.ToString("0");
          
          float randomX = Random.Range(-1f, 1f);
          _currentVelocity = new Vector2(randomX, _startingVelocity);

          Color randColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
          _clickAmountText.color = randColor;
     }

     private void Update()
     {
          _currentVelocity.y -= _velocityDecayRate * Time.deltaTime;
          transform.Translate(_currentVelocity * Time.deltaTime);
          
          _timer += Time.deltaTime;
          if (_timer > _timeBeforeFadeStarts)
          {
               _textAlpha -= _fadeSpeed * Time.deltaTime ;
               _startColor.a = _textAlpha;
               _clickAmountText.alpha = _startColor.a;

               if (_textAlpha <= 0)
               {
                    Destroy(gameObject);
               }
               
          }
     }
}
