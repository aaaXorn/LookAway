using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thrown : MonoBehaviour
{
    public Vector3 StartPos;
    private Vector3 LandPos;

    [SerializeField]
    private ThrownSO t_atk;

    //layer do player
    [SerializeField]
    private LayerMask player_layer;

    private float time;

    private int duration;

    private Rigidbody rigid;

    private bool has_hit, collision_start;

    [SerializeField]
    private GameObject SFX;

    [SerializeField]
    private float disable_time = 1;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        Reset();
        if (SFX != null)
        {
            SFX.transform.parent = null;
            SFX.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (!collision_start)
        {
			StartPos = new Vector3(StartPos.x,
								   transform.position.y,
								   StartPos.z);
            Vector3 MovePos = new Vector3(LandPos.x,
                                          transform.position.y,
                                          LandPos.z);

            //movimento horizontal
            transform.position = Vector3.Lerp(StartPos, MovePos, time / t_atk.h_move_time);

            rigid.AddForce(t_atk.grav, ForceMode.Acceleration);

            //progride o timer com base no tempo desde o �ltimo fixed update
            time += Time.fixedDeltaTime;
        }
        else
        {
            if (duration > 0)
            {
                if (!has_hit)
                {
                    Vector3 pos = transform.position;

                    Collider[] hitCol;

                    //hitbox
                    hitCol = Physics.OverlapSphere(pos, t_atk.radius, player_layer);

                    //dano
                    foreach (var hit in hitCol)
                    {
                        PlayerHealth P_HP = hit.GetComponent<PlayerHealth>();
                        P_HP.TakeDamage(t_atk.dmg);

                        has_hit = true;
                    }
                }

                duration--;
            }
            else
                Reset();
        }
    }

    //volta pro estado inicial
    private void Reset()
    {
        has_hit = false;

        collision_start = false;

        duration = t_atk.duration;

        //tempo de movimento
        time = 0;

        rigid.isKinematic = false;

        gameObject.SetActive(false);
    }

    //quando o objeto � ativado
    private void OnEnable()
    {
        //pega a posi��o atual do jogador
        LandPos = PlayerControl.Instance.transform.position;
		
		if(rigid != null) rigid.AddForce(Vector3.up * t_atk.v_force, ForceMode.Impulse);
    }

    //colocar layer do objeto como Thrown pra colidir s� com Player e Default
    private void OnTriggerEnter(Collider other)
    {
        if (!collision_start)
        {
            collision_start = true;

            if (SFX != null)
            {
                SFX.transform.position = transform.position;
                SFX.SetActive(true);
                Invoke("DisableSFX", disable_time);
            }

            rigid.velocity = Vector3.zero;
            rigid.isKinematic = true;
        }
    }

    private void DisableSFX()
    {
        SFX.SetActive(false);
    }

    #if UNITY_EDITOR
	//efeito visual dentro do editor
	private void OnDrawGizmos()
	{
		if(duration > 0 && collision_start)
			Gizmos.DrawSphere(transform.position, t_atk.radius);
	}
    #endif
}
