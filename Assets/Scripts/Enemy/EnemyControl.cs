using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
	//referência do código do player
	private PlayerControl PlayerC;
	//transform do player
	private Transform PlayerTransf;
	
	//componente do nav mesh
	private NavMeshAgent navAgent;
	
	//componente de vida
	private EnemyHealth E_HP;
	
	//enum com todos os states
	public enum State
	{
		Inactive,//fora de combate
		Active,//inicia o combat
		
		Approach,//indo até o player
		Retreat,//indo para longe do player
		Reposition,//anda até uma posição pré-definida
		Melee,//ataque melee
		Ranged,//ataque ranged
		
		Hurt,//efeito de stun / outros efeitos que se baseiam em tomar dano
		Dead,//quando está morto
		
		Reset//reseta a luta e volta pra posição inicial
	};
	
	//state atual
	public State currentState;
	
	#region attacks
	[Header("Attacks")]
	//layer do player
	[SerializeField]
	private LayerMask player_layer;

	//ID do hit, usado pra mesma hitbox não acertar várias vezes
	private int hit_id;
	private string atk_type;//tipo do ataque
							//número de hits do ataque, hits que tinha quando o ataque começou,
							//último hit dado e hit anterior (pra efeitos que acontecem só uma vez)
	private int atk_hits, curr_hit, prev_hit;
	//dano, duração e tamanho do ataque
	private int[] atk_dmg = new int[5], atk_duration = new int[5], atk_delay = new int[5];
	private int atk_last_frame;
	private float[] atk_size = new float[5], atk_length = new float[5];
	//ponto de origem do ataque
	private Transform[] atk_origin = new Transform[5];
	//movimento durante o ataque
	private float atk_movement;

	//se o ataque pode ser cancelado (geralmente após um hit)
	private bool atk_cancel;
	//se o jogador está atacando
	private bool attacking;

	[System.Serializable]
	//informações dos tipos de ataque
	public class Attack
	{
		/*[Tooltip("Attack's name")]
		public string id;*/

		[Tooltip("Attack properties")]
		public string type;
		[Tooltip("How many hits the attack has")]
		public int hit_count;

		[Tooltip("Attack damage per hit")]
		public int[] dmg;
		[Tooltip("Hitbox duration in frames (24 FPS physics)")]
		public int[] duration;
		[Tooltip("Hitbox radius")]
		public float[] size;
		[Tooltip("Hitbox length, 0 makes it a sphere")]
		public float[] length;
		[Tooltip("Delay between the attacks")]
		public int[] delay;
		[Tooltip("When the attack ends if not canceled")]
		public int last_frame;
		[Tooltip("Attack point of origin")]
		public Transform[] origin;
		
		[Tooltip("Mid attack movement")]
		public float movement;
	}

	//lista com os ataques
	public List<Attack> AtkList;
	//dicionário dos ataques
	//usado para localizar os ataques por string
	//public Dictionary<string, Attack> AtkDictionary;
	//ataque atual
	private int currAtk;
	
	[SerializeField]
	//cooldown do ataque
	private int atk_cd_total;
	private int atk_cd;
	
	[SerializeField]
	//alcance dos ataques
	private float melee_atk_range, ranged_atk_range;
	#endregion
	
	#region movement
	[Header("Movement")]
	//posição inicial
	[SerializeField]
	private Vector3 start_pos;
	
	//velocidade padrão
	[SerializeField]
	private float base_speed;
	//raio da reposição do inimigo
	[SerializeField]
	private float reposition_radius;
	//rng angulo virado pro centro
	#endregion
	
	private void Start()
    {
        //pega o script de controle do jogador
		if(PlayerControl.Instance != null) PlayerC = PlayerControl.Instance;
		else print("PlayerControl Instance not found.");
		//pega o transform do objeto do player
		PlayerTransf = PlayerC.transform;
		
		navAgent = GetComponent<NavMeshAgent>();
		
		E_HP = GetComponent<EnemyHealth>();
		
		atk_cd = atk_cd_total;
		
		start_pos = transform.position;
		
		OnStart();
    }
	
	//funções adicionais do Start, que podem ser adicionadas depois
	protected virtual void OnStart()
	{
		
	}
	
	//ativa o inimigo e começa o combate
	public virtual void Activate()
	{
		if(currentState == State.Inactive || currentState == State.Reset)
		{
			currentState = State.Active;
			
			atk_cd = atk_cd_total;
		}
	}
	//desativa o inimigo, encerrando o combate
	public virtual void Deactivate()
	{
		if(currentState != State.Inactive && currentState != State.Reset && currentState != State.Dead)
		{
			currentState = State.Reset;
			
			//reseta o HP
			E_HP.ResetHP();
			
			navAgent.speed = base_speed;
			
			//muda o alvo pra posição inicial
			navAgent.SetDestination(start_pos);
		}
	}
	
	//state machine
    private void FixedUpdate()
	{
		switch(currentState)
		{
			case State.Inactive:
				StateInactive();
				break;
			
			case State.Active:
				StateActive();
				break;
			
			case State.Approach:
				StateApproach();
				break;
			
			case State.Retreat:
				StateRetreat();
				break;
			
			case State.Reposition:
				StateReposition();
				break;
			
			case State.Melee:
				StateMelee();
				break;
			
			case State.Ranged:
				StateRanged();
				break;
			
			case State.Dead:
				StateDead();
				break;
			
			case State.Reset:
				StateReset();
				break;
			
			default:
				print("Enemy state machine error.");
				break;
		}
	}
	
	#region states
	protected virtual void StateInactive()
	{
		
	}
	
	protected virtual void StateActive()
	{
		//faz o inimigo começar a andar
		navAgent.speed = base_speed;
		currentState = State.Approach;
	}
	
	protected virtual void StateApproach()
	{
		//define o alvo do movimento como o jogador
		navAgent.SetDestination(PlayerTransf.position);
		
		//continua se movendo
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//melee
			if(navAgent.remainingDistance <= melee_atk_range)
			{
				AnimHit(0);
				
				currentState = State.Melee;
			}
			//ranged
			else if(navAgent.remainingDistance <= ranged_atk_range)
			{
				print("ranged");
			}
		}
	}
	
	protected virtual void StateRetreat()
	{
		//vai pra posição oposta da do player
		Vector3 destination = 2 * transform.position - PlayerTransf.position;
		navAgent.SetDestination(destination);
	}
	
	protected virtual void StateReposition()
	{
		
	}
	
	protected virtual void StateMelee()
	{
		//propriedades do ataque
		if (attacking)
		{
			AttackEffect();
		}
		
		//encerra o ataque
		if(atk_last_frame <= 0)
		{
			attacking = false;
			atk_cancel = false;
			
			atk_cd = atk_cd_total;
			
			currentState = State.Approach;
		}
		else atk_last_frame--;
	}
	
	protected virtual void StateRanged()
	{
		
	}
	
	protected virtual void StateDead()
	{
		
	}
	
	protected virtual void StateReset()
	{
		if(navAgent.remainingDistance <= 0.1f)
			currentState = State.Inactive;
	}
	#endregion
	
	#region attacks
	//define o dano, duração, tamanho e tipo da hitbox por ID
	private void AnimHit(int id)//(string id)
	{
		//propriedades do ataque
		//Attack atk = AtkDictionary[id];
		Attack atk = AtkList[id];

		atk_type = atk.type;
		atk_hits = atk.hit_count;
		curr_hit = 0;
		prev_hit = -1;

		for (int i = 0; i < atk_hits; i++)
		{
			atk_dmg[i] = atk.dmg[i];
			atk_duration[i] = atk.duration[i];
			atk_size[i] = atk.size[i];
			atk_length[i] = atk.length[i];
			atk_delay[i] = atk.delay[i];
			atk_last_frame = atk.last_frame;
			atk_origin[i] = atk.origin[i];
		}
		
		atk_movement = atk.movement;
		
		//movimento
		/*if(atk_movement != 0)
		{
			Vector3 transf_f = transform.forward;
			Vector3 direction = new Vector3(transf_f.x, 0, transf_f.z);
			
			rdb.velocity = direction * atk_movement + new Vector3(0, rdb.velocity.y, 0);
		}*/
		navAgent.speed = atk_movement;
		navAgent.SetDestination(PlayerTransf.position);
		
		//inicia o ataque
		attacking = true;
	}

	private void AttackEffect()
	{
		//gera um novo ID pro ataque
		if (prev_hit != curr_hit)
		{
			//encerra os ataques
			if (curr_hit >= atk_hits)
			{
				attacking = false;

				return;
			}
			//continua
			else
			{
				hit_id++;

				prev_hit = curr_hit;
			}
		}

		//após o delay acabar, começa o hit
		if (atk_delay[curr_hit] <= 0)
		{
			//faz o ataque
			if (atk_duration[curr_hit] > 0)
			{
				Vector3 pos = atk_origin[curr_hit].position;

				Collider[] hitCol;

				//hitbox
				hitCol = Physics.OverlapCapsule(pos,
						   pos + atk_origin[curr_hit].forward * atk_length[curr_hit],
						   atk_size[curr_hit], player_layer);

				//dano
				foreach (var hit in hitCol)
				{
					/*EnemyHealth E_HP = hit.GetComponent<EnemyHealth>();
					if (E_HP.hit_id != hit_id)
						E_HP.TakeDamage(atk_dmg[curr_hit]);*/

					atk_cancel = true;

					print("Hit");
				}

				atk_duration[curr_hit]--;
			}
			//encerra esse hit
			else
			{
				curr_hit++;
			}
		}
		//diminui 1 frame do delay
		else atk_delay[curr_hit]--;
	}

	#endregion

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(atk_origin[curr_hit] != null && currentState == State.Melee)
		{
			if(atk_delay[curr_hit] <= 0 && atk_duration[curr_hit] >= 0)
			{
				//desenha a hitbox no editor
				//RIP meu PC
				Vector3 pos = atk_origin[curr_hit].position;
				Vector3 pos2 = pos + atk_origin[curr_hit].forward * atk_length[curr_hit];
				
				// Special case when both points are in the same position
				if (atk_length[curr_hit] == 0)
				{
					// DrawWireSphere works only in gizmo methods
					Gizmos.DrawWireSphere(pos, atk_size[curr_hit]);
					return;
				}
				using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
				{
					Quaternion p1Rotation = Quaternion.LookRotation(pos - pos2);
					Quaternion p2Rotation = Quaternion.LookRotation(pos2 - pos);
					// Check if capsule direction is collinear to Vector.up
					float c = Vector3.Dot((pos - pos2).normalized, Vector3.up);
					if (c == 1f || c == -1f)
					{
						// Fix rotation
						p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
					}
					// First side
					UnityEditor.Handles.DrawWireArc(pos, p1Rotation * Vector3.left,  p1Rotation * Vector3.down, 180f, atk_size[curr_hit]);
					UnityEditor.Handles.DrawWireArc(pos, p1Rotation * Vector3.up,  p1Rotation * Vector3.left, 180f, atk_size[curr_hit]);
					UnityEditor.Handles.DrawWireDisc(pos, (pos2 - pos).normalized, atk_size[curr_hit]);
					// Second side
					UnityEditor.Handles.DrawWireArc(pos2, p2Rotation * Vector3.left,  p2Rotation * Vector3.down, 180f, atk_size[curr_hit]);
					UnityEditor.Handles.DrawWireArc(pos2, p2Rotation * Vector3.up,  p2Rotation * Vector3.left, 180f, atk_size[curr_hit]);
					UnityEditor.Handles.DrawWireDisc(pos2, (pos - pos2).normalized, atk_size[curr_hit]);
					// Lines
					UnityEditor.Handles.DrawLine(pos + p1Rotation * Vector3.down * atk_size[curr_hit], pos2 + p2Rotation * Vector3.down * atk_size[curr_hit]);
					UnityEditor.Handles.DrawLine(pos + p1Rotation * Vector3.left * atk_size[curr_hit], pos2 + p2Rotation * Vector3.right * atk_size[curr_hit]);
					UnityEditor.Handles.DrawLine(pos + p1Rotation * Vector3.up * atk_size[curr_hit], pos2 + p2Rotation * Vector3.up * atk_size[curr_hit]);
					UnityEditor.Handles.DrawLine(pos + p1Rotation * Vector3.right * atk_size[curr_hit], pos2 + p2Rotation * Vector3.left * atk_size[curr_hit]);
				}
			}
		}
	}
#endif
}
