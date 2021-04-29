using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DoctorWolfy121.GravitySystem;

public class PlayerController : MonoBehaviour
{
    private GravityItem playerCharacter;
    public DirectionalGravity primaryGravity;
    public GameObject playerContainer;
    public GameObject tenacleContainer;
    public HintSystem hint;
    public Transform playerIcon;
    private Rigidbody rb;
    public Transform jumpBar;
    public Vector2 speed;
    public float jumpforce;
    public float maxSpeed = 20;
    public void setMaxSpeed(int input)
    {
        maxSpeed = input;
    }
    private int count;

    public TextMeshProUGUI countText;
    //public Text winText;

    public TrailRenderer trailRenderer;
        
    private Collider ballCollider;
    public bool justPlay = false;

    public void playGravitySound()
    {
        GravitySoundSource.PlayOneShot(GravitySound);
    }

    void Start()
    {
        jumpJUICE = jumpforce;
        collected = new List<GameObject>();
        rb = GetComponent<Rigidbody>();
        playerCharacter = GetComponent<GravityItem>();
        ballCollider = GetComponent<Collider>();
        count = 0;

        if (justPlay && Application.isEditor)
        {
            primaryGravity.gravityStrength = 9.8f;
            lockedControl = 0;

        }
        else
        {
            StartCoroutine(StartGame());
        }

            
        //SetCountText();
        //winText.text = "";
    }
    public void playMidRamble()
    {
        if (PlayerPrefs.GetInt("beat") != 1)
        {
            HintSystem.Instance.playTextGeneric(5);
        }
        else
        {
            HintSystem.Instance.playTextGeneric(6);
        }
        
    }


    IEnumerator StartGame()
    {
        hint.playTextGeneric(3);
        rb.isKinematic = false;
        iterateLock(1);
        trailRenderer.enabled = false;
        StartCoroutine(ScaleLerp(playerContainer.transform, playerContainer.transform.localScale.x, 0, 0));

        float currentTime = 0;
        float duration = 15f;
        StartCoroutine(BeginDeform(12f, 0f, 5f, 25f + duration));

        while (currentTime < duration)//delay start by 15 seconds
        {
            currentTime += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(BeginFall(0, 9.8f, 30f));
            
        tenacleContainer.SetActive(true);

        yield return null;
    }
    public void endApplication()
    {
        PlayerPrefs.SetInt("beat", 1);
        Application.Quit();
    }

    public GameObject aspectBlackout;
    public Vector3 NoiseTextureScrollRate = new Vector3(1, 1.1f, 1.05f);
    IEnumerator BeginFall(float start, float finalgravity, float duration)
    {
        aspectBlackout.transform.SetParent(transform.root);
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);

            float scale = Mathf.Lerp(start, finalgravity, amount);

            primaryGravity.gravityStrength = scale;

            yield return null;
        }
        Destroy(aspectBlackout);
            
    }
    IEnumerator BeginDeform(float delay, float start, float endNoiseAmount, float duration)
    {
        Shader.SetGlobalVector("_NoiseScrollRate", NoiseTextureScrollRate);
        float currentTime = 0;
        while (currentTime < delay)//delay start by 15 seconds
        {
            currentTime += Time.deltaTime;

            yield return null;
        }


        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            var amount = Mathf.Lerp(0, 1, (currentTime-delay) / (duration - delay));
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);
            float scale = Mathf.Lerp(start, endNoiseAmount, amount);

            Shader.SetGlobalFloat("_NoiseDisplacePower", scale);


            yield return null;
        }

    }

    //StartCoroutine(ScaleLerp(playerContainer.transform, playerContainer.transform.localScale.x, 1, 5));

    public void upScalePlayer(float duration)
    {
        StartCoroutine(ScaleLerp(playerContainer.transform, playerContainer.transform.localScale.x, 1, duration));
    }
    public void iterateLock(int input)
    {
        lockedControl += input;
    }


    bool inJump = false;

    int canJump = 0;
    Vector3 JumpAxis = Vector3.up;
    int contactCount = 0;

    void OnCollisionEnter(Collision other)
    {

        JumpAxis = Vector3.zero;
        contactCount = 0;
        if (other.gameObject.layer == 8)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                contactCount++;
                JumpAxis += contact.normal;
            }

            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            JumpAxis /= contactCount;
            canJump += 1;
        }


            
        //Debug.Log("JUMP LOCK: " + canJump);
    }
    private void OnCollisionStay(Collision other)
    {
        contactCount = 0;
        if (other.gameObject.layer == 8)
        {
            JumpAxis = Vector3.zero;
            foreach (ContactPoint contact in other.contacts)
            {
                contactCount++;
                JumpAxis += contact.normal;
            }

            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            JumpAxis /= contactCount;

            if (suspendjuice < suspendJuiceFill)
            {
                suspendjuice += Time.fixedDeltaTime;
                jumpBar.localScale = new Vector3(suspendjuice / suspendJuiceFill, 0.023f,1f);
            }

        }
    }

    void OnCollisionExit(Collision other)
    {
        //foreach (ContactPoint contact in other.contacts)
        //{
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
        if (other.gameObject.layer == 8)
        {
            foreach (ContactPoint contact in other.contacts)
            {
                contactCount++;
                JumpAxis += contact.normal;
            }

            //Debug.DrawRay(contact.point, contact.normal, Color.white);
            //Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
            JumpAxis /= contactCount;
            canJump -= 1;
        }
        //canJump = 0;
        Debug.Log("JUMP LOCK: " + canJump);
    }
    bool inJumpHold = false;
    bool inDownHold = false;
    float suspendjuice = 0;
    float suspendJuiceFill = 1.5f;
    int lockedControl = 0;
    bool hasntJumped = true;
    public bool hitHVertical = false;
    float currentJumpForce = 0;
    float jumpJUICE;
    public Vector4 speedXSpeedYStartStop = new Vector4(9f,15f,9f,15f);
    void FixedUpdate()
    {
        if (lockedControl <= 0)
        {
                
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            float absVertical = Mathf.Abs(moveHorizontal);
            float absHorizontal = Mathf.Abs(moveVertical);

            if (absVertical > 0)
            {
                hitHVertical = true;
            }
            if (!hitHVertical)
            {
                //moveVertical = moveHorizontal;
                moveHorizontal = 0;
            }

            if (absHorizontal > 0)
            {
                if (speed.x < speedXSpeedYStartStop.y)
                {
                    speed.x += Time.fixedDeltaTime * 2;
                }
                if (!trailRenderer.enabled)
                {
                    trailRenderer.enabled = true;
                }
            }
            else
            {
                speed.x = speedXSpeedYStartStop.x;
            }
            if (absVertical > 0)
            {
                if (speed.y < speedXSpeedYStartStop.w)
                {
                    speed.y += Time.fixedDeltaTime * 2;
                }
                if (!trailRenderer.enabled)
                {
                    trailRenderer.enabled = true;
                }
            }
            else
            {
                speed.y = speedXSpeedYStartStop.z;
            }


            //Vector3 movement = (Camera.main.transform.right * moveHorizontal) + (Camera.main.transform.forward * moveVertical) + (Vector3.Dot(-playerCharacter.Up, Camera.main.transform.forward) * Camera.main.transform.up * moveVertical).normalized;
            //rb.AddForce(movement * speed);

            float verticality = Vector3.Dot(-playerCharacter.Up, Camera.main.transform.forward); //0 when horizontal, 1 when above, -1 when underneath
            Vector3 movement = (Camera.main.transform.forward * (moveVertical * 2*(1 - Mathf.Abs(verticality))) + (verticality * Camera.main.transform.up * moveVertical));

            rb.AddForce(movement * speed.y);//vertical

            rb.AddForce((Camera.main.transform.right * moveHorizontal) * speed.x);//horizontal
                

            if (canJump > 0 && hasntJumped)
            {
                hint.playSpaceTip();
            }

            if (Input.GetButton("Jump"))
            {
                if (canJump > 0 && !inDownHold && !inJumpHold)
                {
                    //Debug.Log("JUMP");
                    inJumpHold = true;
                    //suspendjuice = suspendJuiceFill;
                        

                    JumpSource.PlayOneShot(JumpSound);
                }
                if (inJumpHold)
                {
                    currentJumpForce = jumpJUICE / 10f;
                        
                    rb.AddForce(JumpAxis * currentJumpForce);
                    jumpJUICE -= currentJumpForce;
                }

                else if (!inJumpHold && canJump == 0 && suspendjuice > 0)
                {
                    hasntJumped = false;
                    //Debug.Log("UP");
                    float suspendJuiceRatio = suspendjuice / suspendJuiceFill;
                    HoverSoundSource.volume = Mathf.Lerp(0f, 1f, suspendJuiceRatio);
                    HoverSoundSource.pitch = Mathf.Lerp(1.5f,0.6f, suspendJuiceRatio);
                    suspendjuice -= Time.fixedDeltaTime/2;
                    jumpBar.localScale = new Vector3(suspendJuiceRatio, 0.023f, 1f);
                    inDownHold = true;
                    rb.AddForce(playerCharacter.Up * jumpforce / 40 * Mathf.Lerp(0.5f,1f, suspendJuiceRatio));
                }
            }
            else
            {
                inJumpHold = false;
                inDownHold = false;
                jumpJUICE = jumpforce;
                HoverSoundSource.volume = 0;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                //Debug.Log("DOWN");
                inDive = true;
                rb.AddForce(-playerCharacter.Up * jumpforce / 50);
                //DiveSoundSource.Play();
                if (!DiveSoundSource.isPlaying)
                {
                    DiveSoundSource.Play();
                }
            }
            else
            {
                inDive = false;
                if (DiveSoundSource.isPlaying)
                {
                    DiveSoundSource.Stop();
                }
            }
                
        }
            

        if (rb.velocity.magnitude > maxSpeed)
        {
            //Debug.Log("MAX SPEED REACHED");
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        playerIcon.transform.rotation = Quaternion.RotateTowards(playerIcon.transform.rotation, Quaternion.LookRotation(rb.velocity.normalized, playerCharacter.Up),5f);
    }
    public bool inDive = false;

    public IEnumerator ScaleLerp(Transform target, float start, float end, float duration)
    {
        float currentTime = 0;
        trailRenderer.widthMultiplier = start;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            var amount = Mathf.Lerp(0, 1, currentTime / duration);
            amount = amount * amount * amount * (amount * (6f * amount - 15f) + 10f);
            float scale = Mathf.Lerp(start, end, amount);
            target.localScale = scale * Vector3.one;

            trailRenderer.widthMultiplier = scale;
            yield return null;
            //yield return new WaitForFixedUpdate();
        }
        trailRenderer.widthMultiplier = end;
        target.localScale = end * Vector3.one;
    }
    List<GameObject> collected;
    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 10 && !collected.Contains(c.gameObject))
        {
            collected.Add(c.gameObject);
            StartCoroutine(Collect(c.gameObject));
        }

    }
    public AudioSource SoundSource;
    public AudioClip CoinSound;
    public AudioClip CoinTenSound;

    public AudioClip JumpSound;
    public AudioClip GravitySound;
    public AudioSource GravitySoundSource;
    public AudioSource JumpSource;
    //public AudioClip HoverSound;
    public AudioSource HoverSoundSource;

    //public AudioClip DivdeSound;
    public AudioSource DiveSoundSource;
    public IEnumerator Collect(GameObject g)
    {
        count++;
        if (count % 10 == 0)
        {
            SoundSource.PlayOneShot(CoinTenSound);
        }
        else
        {
            SoundSource.PlayOneShot(CoinSound);
        }
        float distance = Vector3.Distance(this.transform.position, g.transform.position);
        float currentTime = 0f;
        float duration = 0.25f;
        while (distance > 1f && g !=null && currentTime < duration)
        {
            currentTime += Time.deltaTime;
            //Debug.Log("MOVING CUE");
            g.transform.position = Vector3.MoveTowards(g.transform.position, this.transform.position, 0.2f * distance +.2f);
            distance = Vector3.Distance(this.transform.position, g.transform.position);
            yield return null;
        }

        if (suspendjuice < suspendJuiceFill)
        {
            suspendjuice += suspendJuiceFill/10;
            if (suspendjuice > suspendJuiceFill)
            {
                suspendjuice = suspendJuiceFill;
            }
            jumpBar.localScale = new Vector3(suspendjuice / suspendJuiceFill, 0.023f, 1f);
        }
        Destroy(g.gameObject);
            
        SetCountText();
    }





    void SetCountText()
    {
        if (count >= 100)
        {
            countText.text = "SCORE : " + count.ToString()+" (Wow! You're great!)";
        }
        else
        {
            countText.text = "SCORE : " + count.ToString();
        }
        
        //if (count >= 12)
        //{
        //    winText.text = "You Win!";
        //}
    }
}