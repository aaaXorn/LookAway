using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
	//transform do player
	protected Transform PlayerTransf;
	
	//componente do character controller
	protected CharacterController Control;

	//animação
	protected Animator anim;

	//componente de vida
	protected EnemyHealth E_HP;
	
	//enum com todos os states
	public enum State
	{
		Inactive,//fora de combate
		Active,//inicia o combat
		
		Approach,//indo até o player
		Retreat,//indo para longe do player
		Reposition,//anda até uma posição pré-definida
		Attack,//ataque normal
		Special,//ataque especial
		
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
	protected LayerMask player_layer;

	//se o ataque já acertou o player
	protected bool has_hit;
	protected string atk_type;//tipo do ataque
	//número de hits do ataque, hits que tinha quando o ataque começou,
	//último hit dado e hit anterior (pra efeitos que acontecem só uma vez)
	protected int atk_hits, curr_hit, prev_hit;
	//dano, duração e tamanho do ataque
	protected int[] atk_dmg = new int[5], atk_duration = new int[5], atk_delay = new int[5];
	protected int atk_last_frame;
	protected float[] atk_size = new float[5], atk_length = new float[5];
	//ponto de origem do ataque
	protected Transform[] atk_origin = new Transform[5];
	//movimento durante o ataque
	protected float atk_movement;

	//se o ataque pode ser cancelado (geralmente após um hit)
	protected bool atk_cancel;
	//se o jogador está atacando
	protected bool attacking;

	[System.Serializable]
	//informações dos tipos de ataque
	public class NormalAttack
	{
		[Tooltip("Attack info")]
		public EnemyNormalAttackSO n_atk;
		
		[Tooltip("Attack point of origin")]
		public Transform[] origin;
	}

	//lista com os ataques
	public List<NormalAttack> NAtkList;
	
	[System.Serializable]
	public class SpAttack
	{
		[Tooltip("Attack info")]
		public EnemySpecialAttackSO sp_atk;
		
		[Tooltip("Attack point of origin")]
		public Transform[] origin;
		
		[Tooltip("Projectile manager")]
		public PoolManager Manager;
	}
	
	//manager de pool
	protected PoolManager atk_manager;
	
	//lista com os ataques especiais
	public List<SpAttack> SpAtkList;
	
	//ataque atual
	protected int currAtk, currSpAtk;
	
	[SerializeField]
	//cooldown do ataque
	protected int atk_cd_total;
	protected int atk_cd;
	
	[SerializeField]
	//alcance dos ataques
	protected float melee_atk_range, ranged_atk_range;
	[SerializeField]
	//máximo que o inimigo se aproxima do player no approach state
	protected float max_approach_range;
	#endregion
	
	#region movement
	[Header("Movement")]
	//posição inicial
	[SerializeField]
	protected Vector3 start_pos;
	
	//velocidade padrão
	[SerializeField]
	protected float base_speed;
	protected float speed;
	//raio da reposição do inimigo
	[SerializeField]
	protected int reposition_radius;
	protected Vector3 move_target, move_dir;
	//movimento total da reposição
	[SerializeField]
	protected float reposition_dist;
	//velocidade rotação e rotação durante um ataque
	[SerializeField]
	protected float rot_spd, rot_atk_spd;
	#endregion
	
	protected int pattern;
	
	protected void Start()
    {
		//pega o transform do objeto do player
		PlayerTransf = PlayerControl.Instance.transform;
		
		Control = GetComponent<CharacterController>();

		anim = GetComponent<Animator>();

		E_HP = GetComponent<EnemyHealth>();
		
		atk_cd = atk_cd_total;
		
		start_pos = transform.position;
		
		speed = base_speed;
		
		OnStart();
    }
	
	//funções adicionais do Start, que podem ser adicionadas depois
	protected virtual void OnStart()
	{
		
	}
	
	//ativa o inimigo e começa o combate
	public virtual void Activate()
	{
		if((currentState == State.Inactive || currentState == State.Reset)
		   && currentState != State.Dead)
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
			
			pattern = 0;
			
			//reseta o HP
			E_HP.ResetHP();
		}
	}
	
	//state machine
    protected void FixedUpdate()
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
			
			case State.Attack:
				StateAttack();
				break;
			
			case State.Special:
				StateSpecial();
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

		Vector3 HVelocity = Control.velocity;
		HVelocity = new Vector3(HVelocity.x, 0, HVelocity.z);
		anim.SetFloat("Speed", HVelocity.magnitude);

		//timers, etc
		OnFUpdate();
	}
		protected virtual void OnFUpdate()
		{
			
		}
	
	#region states
	protected virtual void StateInactive()
	{
		
	}
	
	protected virtual void StateActive()
	{
		//faz o inimigo começar a andar
		currentState = State.Approach;
	}
	
	#region movement
	protected virtual void StateApproach()
	{
		Vector3 go_to = PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		
		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		if(go_to.magnitude > max_approach_range)
		{
			//movimento
			Control.SimpleMove(dir * base_speed);
		}
		
		//continua se movendo
		if(atk_cd > 0)
			atk_cd--;
		//ataca
		else
		{
			//distância entre o inimigo e o player
			float dist = go_to.magnitude;
			//melee
			if(dist <= melee_atk_range)
			{
				if(currAtk > NAtkList.Count - 1)
					currAtk = 0;
				
				AnimHit(currAtk);
				
				//muda o próximo ataque
				currAtk++;
				
				currentState = State.Attack;
			}
			//ranged
			else if(dist <= ranged_atk_range)
			{
				if(currSpAtk > SpAtkList.Count - 1)
					currSpAtk = 0;
				
				SpecialHit(currSpAtk);
				
				//muda o próximo ataque
				currSpAtk++;
				
				currentState = State.Special;
			}
		}
	}
	
	protected virtual void StateRetreat()
	{
		//pega a direção oposta do player
		Vector3 dir = (transform.position - PlayerTransf.position).normalized;
		
		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		//move o inimigo
		Control.SimpleMove(dir * base_speed);
	}
	
	protected virtual void StateReposition()
	{
		//movimento
		Control.SimpleMove(move_dir * base_speed);

		//rotação
		Quaternion rot = Quaternion.LookRotation(move_dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_spd);

		//quando chega no ponto escolhido, para
		if (Vector3.Distance(transform.position, move_target) <= 1)
			currentState = State.Active;
	}
		//define pra qual posição o reposition vai ir
		protected virtual void RepositionStart()
		{
			//aleatoriza a direção
			float rad = Random.Range(0, reposition_radius + 1);
			rad -= reposition_radius/2;
			
			//direção
			Vector3 go_to = start_pos - transform.position;
			move_target = Quaternion.AngleAxis(rad, Vector3.up) * go_to;
			move_dir = move_target.normalized;
			//posição final
			move_target = transform.position + (move_dir * reposition_dist);
			
			currentState = State.Reposition;
		}
	#endregion
	
	protected virtual void StateAttack()
	{
		//propriedades do ataque
		if (attacking)
		{
			AttackEffect();
		}
		
		//movimento
		if (atk_movement != 0)
		{
			Vector3 go_to = PlayerTransf.position - transform.position;
			//direção
			Vector3 dir = go_to.normalized;
			
			if(atk_duration[curr_hit] > 0 && go_to.magnitude > max_approach_range)
				Control.SimpleMove(transform.forward * atk_movement);

			//rotação
			Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
			rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_atk_spd);
		}
		
		//encerra o ataque
		if (atk_last_frame <= 0)
		{
			attacking = false;
			atk_cancel = false;

			anim.SetTrigger("Free");

			PostAttackState();
		}
		else atk_last_frame--;
	}
		protected virtual void PostAttackState()
		{
			RepositionStart();
			//currentState = State.Approach;
		}
	
	protected virtual void StateSpecial()
	{
		if(attacking)
		{
			SpAttackEffect();
		}
		
		//movimento
		if(atk_movement != 0)
			Control.SimpleMove(transform.forward * atk_movement);

		Vector3 go_to = PlayerTransf.position - transform.position;
		//direção
		Vector3 dir = go_to.normalized;

		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_atk_spd);

		//encerra o ataque
		if (atk_last_frame <= 0)
		{
			attacking = false;
			atk_cancel = false;

			anim.SetTrigger("Free");

			PostSpecialState();
		}
		else atk_last_frame--;
	}
		protected virtual void PostSpecialState()
		{
			//RepositionStart();
			currentState = State.Approach;
		}
	
		public virtual void Hurt(int hp, int max_hp)
		{
			
		}
	
	protected virtual void StateDead()
	{
		
	}
		public virtual void Dead()
		{
			anim.SetTrigger("Dead");
			
			currentState = State.Dead;
		}
	
	protected virtual void StateReset()
	{
		Vector3 go_to = start_pos - transform.position;
		//direção
		Vector3 dir = go_to.normalized;
		//distância
		float dist = go_to.magnitude;
		
		//move o inimigo
		Control.SimpleMove(dir * base_speed);

		//rotação
		Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
		rot = new Quaternion(transform.rotation.x, rot.y, transform.rotation.z, rot.w);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rot_atk_spd);

		if (dist <= 0.1f)
			currentState = State.Inactive;
	}
	#endregion
	
	#region attacks
	//define o dano, duração, tamanho e tipo da hitbox por ID
	protected void AnimHit(int id)
	{
		anim.SetInteger("AtkID", id);
		anim.SetTrigger("NormalAtk");

		//propriedades do ataque
		NormalAttack atk = NAtkList[id];
		EnemyNormalAttackSO n_atk = atk.n_atk;
		
		atk_hits = n_atk.hit_count;
		curr_hit = 0;
		prev_hit = -1;

		for (int i = 0; i < atk_hits; i++)
		{
			atk_dmg[i] = n_atk.dmg[i];
			atk_duration[i] = n_atk.duration[i];
			atk_size[i] = n_atk.size[i];
			atk_length[i] = n_atk.length[i];
			atk_delay[i] = n_atk.delay[i];
			atk_origin[i] = atk.origin[i];
		}
		
		atk_movement = n_atk.movement;
		atk_last_frame = n_atk.last_frame;
		atk_cd = n_atk.cooldown;
		
		//inicia o ataque
		attacking = true;
	}
	
	protected void SpecialHit(int id)
	{
		anim.SetInteger("AtkID", id);
		anim.SetTrigger("SpecialAtk");

		//propriedades do ataque
		SpAttack atk = SpAtkList[id];
		
		EnemySpecialAttackSO sp_atk = atk.sp_atk;
		
		atk_type = sp_atk.type;
		atk_hits = sp_atk.hit_count;
		curr_hit = 0;
		prev_hit = -1;

		for (int i = 0; i < atk_hits; i++)
		{
			atk_delay[i] = sp_atk.delay[i];
			atk_origin[i] = atk.origin[i];
		}
		
		atk_movement = sp_atk.movement;
		atk_last_frame = sp_atk.last_frame;
		atk_cd = sp_atk.cooldown;
		
		atk_manager = atk.Manager;
		
		//inicia o ataque
		attacking = true;
	}

	protected void AttackEffect()
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
				has_hit = false;

				prev_hit = curr_hit;
			}
		}

		//após o delay acabar, começa o hit
		if (atk_delay[curr_hit] <= 0)
		{
			//faz o ataque
			if (atk_duration[curr_hit] > 0)
			{
				if(!has_hit)
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
						PlayerHealth P_HP = hit.GetComponent<PlayerHealth>();
						P_HP.TakeDamage(atk_dmg[curr_hit]);
						
						has_hit = true;
						
						//atk_cancel = true;
					}
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
	
	protected void SpAttackEffect()
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
				has_hit = false;

				prev_hit = curr_hit;
			}
		}
		
		//após o delay acabar, começa o hit
		if (atk_delay[curr_hit] <= 0)
		{
			//cria o projétil
			GameObject obj = atk_manager.GetFromPool();
			
			if(obj == null)
			{
				print("Special attack prefab is null.");
				
				return;
			}
			
			//configura o projétil
			obj.SetActive(true);
			
			//muda a posição/rotação/etc dependendo do tipo de ataque
			AtkTypeSwitch(obj);
			
			curr_hit++;
		}
		//diminui 1 frame do delay
		else atk_delay[curr_hit]--;
		
	}
		protected virtual void AtkTypeSwitch(GameObject obj)
		{
			switch(atk_type)
			{
				case "Shockwave":
					obj.transform.position = atk_origin[curr_hit].position;
					obj.transform.rotation = atk_origin[curr_hit].rotation;
					break;

				case "Thrown":
					obj.transform.position = atk_origin[curr_hit].position;
					obj.transform.LookAt(PlayerControl.Instance.transform.position);
					obj.GetComponent<Thrown>().StartPos = atk_origin[curr_hit].position;
					break;
					
				case "Laser":
					obj.transform.position = atk_origin[curr_hit].position;
					obj.transform.LookAt(PlayerControl.Instance.transform.position);
					break;
				
				case "Spikes":
					
					break;
				
				default:
					obj.transform.position = atk_origin[curr_hit].position;
					obj.transform.rotation = atk_origin[curr_hit].rotation;
					break;
			}
		}
	#endregion

#if UNITY_EDITOR
	protected void OnDrawGizmos()
	{
		if(atk_origin[curr_hit] != null && currentState == State.Attack)
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
