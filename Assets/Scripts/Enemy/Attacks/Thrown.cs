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
    protected LayerMask player_layer;

    private float time;

    private int duration;

    private Rigidbody rigid;

    private bool has_hit, collision_start;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        Reset();
    }

    private void FixedUpdate()
    {
        if (!collision_start)
        {
            Vector3 MovePos = new Vector3(LandPos.x,
                                          transform.position.y,
                                          LandPos.z);

            //movimento horizontal
            transform.position = Vector3.Lerp(StartPos, MovePos, time / t_atk.h_move_time);

            rigid.AddForce(t_atk.grav);

            //progride o timer com base no tempo desde o último fixed update
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

    //quando o objeto é ativado
    private void OnEnable()
    {
        //pega a posição atual do jogador
        LandPos = PlayerControl.Instance.transform.position;
    }

    //colocar layer do objeto como Thrown pra colidir só com Player e Default
    private void OnTriggerEnter(Collider other)
    {
        if (!collision_start)
        {
            collision_start = true;

            rigid.isKinematic = true;
        }
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
