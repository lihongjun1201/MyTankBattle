using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 控制血条UI的变化
/// 利用color.lerp函数在绿与红之间随着血量比例渐变
/// 并且控制死亡坦克爆炸粒子效果的产生和销毁
/// 控制爆炸时的音效
/// 
/// </summary>


public class TankHealthController : MonoBehaviour {

    public float m_BeginHealth = 100f;

    public Slider m_HealthSlider;
    public Image m_FillImage;
    public Color m_FullColor = Color.green;
    public Color m_ZeroColor = Color.red;

    //the tank Explosion particle prefab 
    //contains soundclip  & particles
    public GameObject m_ExplosionPrefab;      

    private AudioSource m_AudioSource;       //the sound of Tank-Explosion prefab
    private ParticleSystem m_ExplosionParticle;

    private float m_CurrentHealth;
    private bool m_IsDead;
    
    private void Awake()
    {
        m_ExplosionParticle = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_AudioSource = m_ExplosionParticle.GetComponent<AudioSource>();
        m_ExplosionParticle.gameObject.SetActive(false);
    }

    //when the tank is enable
    private void OnEnable()
    {
        m_IsDead = false;
        m_CurrentHealth = m_BeginHealth;
    }

    // Use this for initialization
    void Start () {
        changeHealthUI();
    }
	
	// Update is called once per frame
	void Update () {
       

    }

    public void BeAttacked(float damage)
    {
        m_CurrentHealth -= damage;
        changeHealthUI();

        if (m_CurrentHealth <= 0f && !m_IsDead)
        {
            OnDeath();
        }
    }

    public void changeHealthUI() 
    {
        m_HealthSlider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroColor,m_FullColor, m_CurrentHealth/m_BeginHealth);
    }

    /// <summary>
    /// set the tankExplosion prefeb at the point of the tank,set it visable
    /// play the soundclip
    /// play the particle effects
    /// disvisible the tank
    /// 
    /// </summary>
    private void OnDeath()
    {
        m_IsDead = true;

        m_ExplosionParticle.transform.position = gameObject.transform.position;
        m_ExplosionParticle.gameObject.SetActive(true);
        m_ExplosionParticle.Play();

        m_AudioSource.Play();

        gameObject.SetActive(false);
    }


}
