using UnityEngine;
using System.Collections;

/// <summary>
/// 1 控制子弹的生成时机，播放子弹爆炸例子特效 && 子弹爆炸音效，控制子弹的存在时长并销毁
/// 2 控制子弹爆炸的影响范围，并且调用坦克的BeAttacked函数进行扣血，在范围中心的扣血多，边缘则少，外则无
/// 3 子弹击中坦克爆炸后会施力导致坦克移位
/// 4.实现OnTriggerEnter函数
/// </summary>


public class ShellExplosionScript : MonoBehaviour {
    
    //explosion only effects the tanks layer
    public LayerMask m_TankMask;

    //paticleSys
    public ParticleSystem m_shellExplosionParticle;
    //AudioSource
    public AudioSource m_AudioSource;

    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_LifeTime = 2f;
    public float m_ExplosionRadius = 5f;
    
	// Use this for initialization
	void Start () {
       
        Destroy(gameObject, m_LifeTime);
    }

    private void OnTriggerEnter()
    {
        

        //return a array contains all colliders in the shell's sphere collider.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius,m_TankMask);

        
        for (int i = 0; i < colliders.Length; i++)
        {
            // use rigidbody to make the tank move by the shellforce
            Rigidbody tankRigidBody = colliders[i].GetComponent<Rigidbody>();
            //if it is other things but not the tank,ignore
            if (!tankRigidBody)
                continue;
            //add explosion force to simulate the tank hit by the shell
            tankRigidBody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            float realDamage = getDamageByPosition(tankRigidBody.position);
            //call the tankHealth script to minus the tank's health
            TankHealthController tankHealthcode = tankRigidBody.GetComponent<TankHealthController>();
            if (!tankHealthcode)
                continue;
            tankHealthcode.BeAttacked(realDamage);

            //disattach the explosion Particle to its parent
            m_shellExplosionParticle.transform.parent = null;

            //play paticle and sound effects;
            m_shellExplosionParticle.Play();
            m_AudioSource.Play();

            //when the lifetime of the particle ends,destroy it;
            Destroy(m_shellExplosionParticle.gameObject, m_shellExplosionParticle.duration);
            Destroy(gameObject);
        }
    }

    private float getDamageByPosition(Vector3 tankPos)
    {
        Vector3 exlposionToTank = tankPos - transform.position;
        float realDistance = exlposionToTank.magnitude;
        float effectRatio = (m_ExplosionRadius - realDistance) / m_ExplosionRadius;
        float realDamage = effectRatio * m_MaxDamage;
        return Mathf.Max(0f, realDamage); ;
    }

    

}
