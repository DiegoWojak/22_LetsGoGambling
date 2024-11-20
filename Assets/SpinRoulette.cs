using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpinRoulette : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    [SerializeField]
    private int lenghtlist = 0;
    [SerializeField]
    private bool allStartInmediatly;
    [SerializeField] 
    private float radius;
    
    [SerializeField]
    private RouletteConfig[] roulettesTransform;

    private int _child;
    private bool isEnque = false;
    private bool isRquestComplete = true;
    public static Action OnSpinRequestedStarted;
    public static Action OnSpinStarted;
    public static Action OnSpinRequestedEnd;
    public static Action OnSpinEnd;

    [Serializable]
    public class RouletteConfig
    {
        public Transform transform;
        public bool isSpinning { get { return (spingRef!=null && LeanTween.isTweening(spingRef.id)); } }
        public LTDescr spingRef;
        public float lastRotation;
    }

    private void SetReq(bool _req)
    {
        isEnque = _req;
        Debug.Log($"Status of Sping : {isEnque}");
    }

    void Start()
    {
        foreach (RouletteConfig t in roulettesTransform)
        {
            SpriteRenderer spriteRenderer = null;
            for (int i = 0; i < lenghtlist; i++)
            {
                var _sprite = GetAny();
                spriteRenderer = new GameObject(name: _sprite.name).AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = _sprite;
                spriteRenderer.transform.SetParent(t.transform);
            
                spriteRenderer.transform.localScale = Vector3.one * 2.5f;
            }

            _child = t.transform.childCount;
            //offsetRotation = 360.0f / _child;

            for (int i = 0;i < _child; i++)
            {
                float angle = i * Mathf.PI * 2f / _child;
                Vector3 newPos = new Vector3(0, Mathf.Sin(angle) * radius, -Mathf.Cos(angle) * radius);
                t.transform.GetChild(i).localPosition = newPos;
            }
        }
    }

    void OnEnable()
    {
        OnSpinRequestedStarted += () => { SetReq(true); };
        OnSpinRequestedEnd += () => { isRquestComplete = false; };
        OnSpinEnd += () => { isRquestComplete = true; SetReq(false); };
    }

    void OnDisable()
    {

    }

    Sprite GetAny() { 
        return sprites[UnityEngine.Random.Range(0,sprites.Count)]; 
    }

    [ContextMenu("Test spin")]    
    private void MakeItSpin()
    {
        if (isEnque) return;
        OnSpinRequestedStarted?.Invoke();
        StartCoroutine(Enque());

        OnSpinStarted?.Invoke();
    }

    IEnumerator Enque() {
        foreach (var t in roulettesTransform)
        {
            yield return StartCoroutine(Spin(t));
        }
    }

    

    IEnumerator Spin(RouletteConfig _ref)
    {
        Transform _refto = _ref.transform;
        LTDescr vr;
        
        WaitForSeconds _time = allStartInmediatly ? new WaitForSeconds(0f) : new WaitForSeconds(0.5f);
        yield return _time;

        float t = 2.5f;
        vr = LeanTween.rotateAround(_refto.gameObject, Vector3.right, 360, t);

        vr.setOnUpdate((float val) =>
        {
            for (int i = 0; i < _child; i++)
            {
                _refto.GetChild(i).rotation = Quaternion.identity;
            }

            if(val > 150.9f)
            {
                if(t > 1f)
                {
                    t -= 0.05f;
                    vr.setTime(t);
                }
            }
            _ref.lastRotation = val;
        }).setLoopCount(-1);

        _ref.spingRef = vr;
    }

    [ContextMenu("Test Stop")]
    public void MakeItStop()
    {
        if (!isEnque || !isRquestComplete) return;
        OnSpinRequestedEnd?.Invoke();

        for (int i=0; i<roulettesTransform.Length;i++) {
            StopSpin(roulettesTransform[i],i== roulettesTransform.Length-1, OnSpinEnd);
        }
    }

    private void StopSpin(RouletteConfig _ref, bool isLast = false, Action OnLast = null)
    {
        Debug.Log("Make it Stop");
        Transform _refto = (_ref.transform);
        int vr = _ref.spingRef.id;

        LeanTween.cancel(vr);

        float offsetRotation = 360.0f / _child;
        float lastRotation = _ref.lastRotation;
        float lowerBound = Mathf.Floor(lastRotation / offsetRotation) * offsetRotation;
        float upperBound = lowerBound + offsetRotation;

        // Determine which bound is closer
        float closestTarget;
        if (Mathf.Abs(lastRotation - lowerBound) <= Mathf.Abs(lastRotation - upperBound))
        {
            closestTarget = lowerBound; // Snap to the lower bound
        }
        else
        {
            closestTarget = upperBound; // Snap to the upper bound
        }

        float time = UnityEngine.Random.Range(0.5f, 1.5f);

        LeanTween.rotateAround(_refto.gameObject, Vector3.right, (closestTarget-lastRotation), time).setOnUpdate((float val) => {
            ChildUniform(val, _refto);
            }
        ).setEaseInOutBounce().setOnComplete(() =>
        {
            Debug.Log($"lower bound {closestTarget} upperbound {upperBound}");
            if(isLast) OnLast?.Invoke();
        });
    }

    void ChildUniform(float val, Transform parent)
    {
        for (int i = 0; i < _child; i++)
        {
            parent.GetChild(i).rotation = Quaternion.identity;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            MakeItSpin();
        }

        if (Input.GetMouseButtonDown(0))
        {
            MakeItStop();
        }
    }
}
