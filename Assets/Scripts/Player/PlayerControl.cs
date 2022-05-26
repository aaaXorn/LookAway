using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
	//referência global do código do personagem
	public static PlayerControl Instance {get; set;}

	#if UNITY_EDITOR
	[SerializeField]
	private bool debug_mode;
	[SerializeField]
	private int debug_fps;
	#endif

	//referência do joystick de movimento
	private MovementJoystick MoveJ;
	//referência do script de HP
	private PlayerHealth P_HP;
	//referência do script de camera lock
	private CamLock CL;

	//enum com os states do player
	private enum State
	{
		Free,//movimento e pulo
		Attack,//atacando
		Block,//bloqueando
		Roll,//desviando
		
		Hurt,//tomando hit
		Dead//morrendo
	};
	
	//frames de buffer
	private int buffer_f;
	[SerializeField]
	private int buffer_f_total;
	
	//state atual do player
	[SerializeField]
	private State currentState;

	#region movement
	[Header("Movement")]
    public Rigidbody rdb;
    public Animator anim;
    private Vector3 movaxis, turnaxis;
    public GameObject currentCamera;
	public float movespeed = 5;
    public float jumpspeed = 8;
    public float gravity = 20;
	
    public float jumptime;
    private float flyvelocity = 3;
    public GameObject wing;
    public Transform rightHandObj, leftHandObj;
    private bool jumpbtn = false;
    private bool jumpbtndown = false;
    private bool jumpbtnrelease = false;
    private GameObject closeThing;
    private float weight;

	private bool attackbtn = false;
	private bool blockbtn = false;
	private bool rollbtn = false;
	#endregion

	#region attacks
	[Header("Attacks")]
	//layer dos inimigos
	[SerializeField]
	private LayerMask enemy_layer;
	
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
	#endregion

	#region defenses
	[Header("Defenses")]
	
	//tempo de invul atual
	private int invul_f;
	
	//tempo de invul block em frames
	[Tooltip("Frames in 24 FPS (FixedUpdate FPS)")]
	[SerializeField]
	private int block_f_total;
	
	//tempo de invul do roll em frames e tempo da animação
	[SerializeField]
	private int roll_f_total, roll_anim_f_total;
	private int roll_anim_f;
	[SerializeField]
	private float rollspeed;
	
	//tempo de invul depois de tomar dano e tempo da animação
	[SerializeField]
	private int hurt_f_total, hurt_anim_f_total;
	private int hurt_anim_f;
	//número de animações de tomar dano
	[SerializeField]
	private int hurt_animations;
	
	//tempo até a tela de game over aparecer
	[SerializeField]
	private int dead_f;
	#endregion

	private void Awake()
	{
		//setta a referência global desse script
		if(Instance == null) Instance = this;
		//garante que só tem um dele na cena
		else Destroy(gameObject);
	}
	
    // Start is called before the first frame update
    private void Start()
    {
		//pega o script de movimento com joystick
		if(MovementJoystick.Instance != null) MoveJ = MovementJoystick.Instance;
		else print("MovementJoystick Instance not found.");
		//pega o script de movimento com joystick
		if (PlayerHealth.Instance != null) P_HP = PlayerHealth.Instance;
		else print("PlayerHealth Instance not found.");
		//pega o script de movimento com joystick
		if (CamLock.Instance != null) CL = CamLock.Instance;
		else print("CamLock Instance not found.");
		
        currentCamera = Camera.main.gameObject;
	   
	    //cria o dicionário de ataques
		/*AtkDictionary = new Dictionary<string, Attack>();
		foreach(Attack atk in AtkList)
		{
			AtkDictionary.Add(atk.id, atk);
		}*/
		
		#if UNITY_EDITOR
		if(debug_mode) Application.targetFrameRate = debug_fps;
		#endif
    }

    private void Update()
    {
		//inputs de debugging
		#if UNITY_EDITOR
        if(Input.GetButtonDown("Jump"))
        {
            JumpDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            JumpUp();
        }
		if(Input.GetButtonDown("Fire1"))
		{
			AttackDown();
		}
		else if(Input.GetButtonDown("Fire2"))
		{
			BlockDown();
		}
		else if(Input.GetButtonDown("Fire3"))
		{
			RollDown();
		}
		
		if(Input.GetButtonUp("Fire2"))
		{
			BlockUp();
		}
		//movaxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		#endif

		//input de pulo está no script JumpButton
		//input de ataque está no script AttackButton

		//movimento com o joystick
		//.normalized garante que o jogador sempre ande na mesma velocidade
		movaxis = new Vector3(MoveJ.Horizontal, 0, MoveJ.Vertical);
    }

	//state machine
    void FixedUpdate()
    {
		//state machine
		switch(currentState)
		{
			//movimentação e pulo
			case State.Free:
				StateFree();
				break;
			
			//ataque
			case State.Attack:
				StateAttack();
				break;
			
			//bloqueio
			case State.Block:
				StateBlock();
				break;
			
			//desvio
			case State.Roll:
				StateRoll();
				break;
			
			//tomando dano
			case State.Hurt:
				StateHurt();
				break;
			
			//morrendo
			case State.Dead:
				StateDead();
				break;
			
			default:
				print("Player state machine error.");
				break;
		}
		
		//reseta os button down
        jumpbtndown = false;
		
		//buffer
		if(buffer_f > 0)
			buffer_f--;
		else if(buffer_f == 0)
		{
			//para só rodar uma vez
			buffer_f--;
			
			attackbtn = false;
			rollbtn = false;
        }
		
		//== para só rodar uma vez
		if(invul_f == 0)
        {
			//termina os invulframes
			P_HP.invul = false;
			
			invul_f--;
        }
		else if(invul_f > 0)
			invul_f--;
    }

	#region states
    private void StateFree()
    {
		#region movement
        //define a direção relativa que o player vai andar
		//.normalized pra ficar sempre na vel máxima
        Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis.normalized);
		relativedirection = new Vector3(relativedirection.x, jumptime, relativedirection.z);
		//mesma coisa só que sem a coordenada Y
		Vector3 relativeDirectionWOy = new Vector3(relativedirection.x, 0, relativedirection.z);

		//* 5/6 da uma suavizada na animação, necessário por causa da diminuição do FPS do FixedUpdate
		//sem isso o jogador move as pernas rápido demais em comparação com o quanto ele anda
		anim.SetFloat("Speed", rdb.velocity.magnitude * 5 / 6);

		//movimento com o glider
		if (wing.activeSelf)
		{
			float velocity = Mathf.Abs(rdb.velocity.x) + Mathf.Abs(rdb.velocity.z);
			velocity = Mathf.Clamp(velocity, 0, 2 * movespeed);

			rdb.AddRelativeForce(new Vector3(0, velocity * 120, 1000));

			Vector3 movfly = new Vector3(Vector3.forward.x * flyvelocity, 0, Vector3.forward.z * flyvelocity);

			float angz = Vector3.Dot(transform.right, Vector3.up);
			float angx = Vector3.Dot(transform.forward, Vector3.up);//*50;
			movfly = new Vector3(movaxis.z + angx * 2, -angz, -movaxis.x - angz);
			
			transform.Rotate(movfly);

			wing.transform.localRotation = Quaternion.Euler(0, 0, angz * 50);


			flyvelocity -= angx * 0.01f;
			flyvelocity = Mathf.Lerp(flyvelocity, 3, Time.fixedDeltaTime);
			flyvelocity = Mathf.Clamp(flyvelocity, 0, movespeed);

		}
		//movimento sem o glider
		else
		{
			//versão com velocity
			rdb.velocity = relativeDirectionWOy * movespeed + new Vector3(0, rdb.velocity.y, 0);
			//versão do commit do reinaldo, com addforce
			//não utilizado por falta de precisão, o outro parece mais com o movimento do Monster Hunter
			//rdb.AddForce(relativeDirectionWOy * 10000 * movespeed/(rdb.velocity.magnitude+1));
			Quaternion rottogo = Quaternion.LookRotation(relativeDirectionWOy * 2 + transform.forward);
			transform.rotation = Quaternion.Lerp(transform.rotation, rottogo, Time.fixedDeltaTime * 50);
		}
		#endregion

		#region jump and actions
		//checa se da pra pular
		RaycastHit hit;
		if (Physics.Raycast(transform.position - (transform.forward * 0.1f) + transform.up * 0.3f, Vector3.down, out hit, 1000))
		{
			anim.SetFloat("JumpHeight", hit.distance);

			//se está no chão/quase no chão
			if (hit.distance < 0.5f)
			{
				#region actions
				//se o botão foi pressionado
				//ataque
				if (attackbtn)
				{
					AnimAttack();
				}
				//block
				else if (blockbtn)
				{
					AnimBlock();
				}
				//roll
				else if (rollbtn)
				{
					AnimRoll();
				}
				//deixa o jogador começar o pulo / hold jump
				else if(jumpbtn)
					jumptime = 0.25f;
				#endregion
			}

			//ativa as asas
			if (hit.distance > 0.5f && jumpbtndown && !wing.activeSelf)
			{
				wing.SetActive(true);
				jumpbtndown = false;
			}
			//desativa as asas
			else if (hit.distance > 0.5f && jumpbtndown && wing.activeSelf)
			{
				wing.SetActive(false);
			}
		}

		//pulo
		if (jumpbtn)
		{
			jumptime -= Time.fixedDeltaTime;
			jumptime = Mathf.Clamp01(jumptime);
			rdb.AddForce(Vector3.up * jumptime * jumpspeed);
		}
		#endregion
	}

	private void StateAttack()
    {
		//propriedades do ataque
		if (attacking)
		{
			AttackEffect();
		}
		
		//se o ataque pode ser cancelado
		if (atk_cancel)
		{
			//se o botão de ataque foi pressionado
			if (attackbtn && AtkList.Count > currAtk + 1)
			{
				anim.SetTrigger("Attack");
				
				currAtk++;
				
				AnimHit(currAtk);
				
				atk_cancel = false;
			}

			//cancels de movimento/defesa
			else if(rollbtn)
			{
				#region rotação roll
				//define a direção relativa que o player vai andar
				//.normalized pra ficar sempre na vel máxima
				Vector3 relativedirection = currentCamera.transform.TransformVector(movaxis.normalized);
				relativedirection = new Vector3(relativedirection.x, jumptime, relativedirection.z);
				//mesma coisa só que sem a coordenada Y
				Vector3 relativeDirectionWOy = new Vector3(relativedirection.x, 0, relativedirection.z);
				
				Quaternion rottogo = Quaternion.LookRotation(relativeDirectionWOy * 2 + transform.forward);
				transform.rotation = rottogo;
				#endregion
				
				AnimRoll();
				
				atk_cancel = false;
			}
			else if (blockbtn)
			{
				AnimBlock();
				
				atk_cancel = false;
			}
			else if (jumpbtn)
			{
				//deixa o jogador pular
				jumptime = 0.25f;

				//volta pro estado normal
				currentState = State.Free;
				
				atk_cancel = false;
			}
		}
		
		//encerra o ataque
		if(atk_last_frame <= 0)
			AnimFree();
		else atk_last_frame--;
	}

	private void StateBlock()
    {
		//sai do estado de block
		if(!blockbtn)
		{
			anim.SetBool("Block", false);
			P_HP.blocking = false;
			
			currentState = State.Free;
		}
		
		//cancela o block com um roll
		if(rollbtn)
		{
			anim.SetBool("Block", false);
			P_HP.blocking = false;
			
			AnimRoll();
		}
    }

    private void StateRoll()
    {
		//movimento em AnimRoll
		
		//determina quando o roll acaba
		if(roll_anim_f > 0)
			roll_anim_f--;
		else AnimFree();
	}
	
	private void StateHurt()
	{
		if (hurt_anim_f > 0)
			hurt_anim_f--;
		else
			AnimFree();
	}
	
	private void StateDead()
	{
		//timer até o player morrer, menu de morte
		if(dead_f > 0)
			dead_f--;
		else
			PauseMenu.Instance.GameOverScreen();
	}
    #endregion

    #region animation
    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (wing.activeSelf)
        {

            if (rightHandObj != null)
            {
               
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);

            }
        }
		
		//animação de empurrar, removida
		//quando tem um objeto empurrado
        /*if (closeThing)
        {
			//cancela a animação se estiver em um ataque/roll/etc
			if(currentState != State.Free)
			{
				Destroy(closeThing);
				
				return;
			}
			
			//calcula a direcao do ponto de toque para a personagem
			Vector3 handDirection = closeThing.transform.position - transform.position;
			//verifica se o objeto ta na frente do personagem >0
			float lookto = Vector3.Dot(handDirection.normalized, transform.forward);
			//calcula e interpola o peso pela formula (l*3)/distancia^3
			weight=Mathf.Lerp(weight,(lookto*3 / (Mathf.Pow(handDirection.magnitude,3))),Time.fixedDeltaTime*2);
		   
			anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
			anim.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);
			anim.SetIKPosition(AvatarIKGoal.RightHand, closeThing.transform.position + transform.right * 0.1f);
			anim.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.identity);

			anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
			anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weight);
			anim.SetIKPosition(AvatarIKGoal.LeftHand, closeThing.transform.position- transform.right*0.1f);
			anim.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.identity);
			
            if (weight <= 0)
            {
                Destroy(closeThing);
            }
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        wing.SetActive(false);
        /*if (collision.transform.position.y > transform.position.y + .05f) {
            if(!closeThing)
            closeThing = new GameObject("Handpos");

            weight = 0;
            closeThing.transform.parent = collision.gameObject.transform;
            closeThing.transform.position= collision.GetContact(0).point;
        }*/
    }
	
    /*private void OnCollisionExit(Collision collision)
    {
		
    }*/
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
		
		for(int i = 0; i < atk_hits; i++)
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
		
		//inicia o ataque
		attacking = true;
		
		//rotaciona o player na direção do inimigo
		if(CL.cam_lock)
		{
			Vector3 dir = CL.cam_target[CL.curr_target].position - transform.position;
			Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
			transform.rotation = rot;
		}
		
		//movimento
		if(atk_movement != 0)
		{
			Vector3 transf_f = transform.forward;
			Vector3 direction = new Vector3(transf_f.x, 0, transf_f.z);
			
			rdb.velocity = direction * atk_movement + new Vector3(0, rdb.velocity.y, 0);
		}
	}
	
	private void AttackEffect()
	{
		//gera um novo ID pro ataque
		if(prev_hit != curr_hit)
		{
			//encerra os ataques
			if(curr_hit >= atk_hits)
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
		if(atk_delay[curr_hit] <= 0)
		{
			#if UNITY_EDITOR
			if(debug_mode) atk_cancel = true;
			#endif
			
			//faz o ataque
			if(atk_duration[curr_hit] > 0)
			{
				Vector3 pos = atk_origin[curr_hit].position;
				
				Collider[] hitCol;
				
				//hitbox
				hitCol = Physics.OverlapCapsule(pos,
						   pos + atk_origin[curr_hit].forward * atk_length[curr_hit],
						   atk_size[curr_hit], enemy_layer);
			
				//dano
				foreach(var hit in hitCol)
				{
					EnemyHealth E_HP = hit.GetComponent<EnemyHealth>();
					if(E_HP.hit_id != hit_id)
					{
						E_HP.hit_id = hit_id;
						E_HP.TakeDamage(atk_dmg[curr_hit]);
					}
					
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

	#region animation events
	//volta pro State Free
	private void AnimFree()
	{
		currentState = State.Free;

		attacking = false;
		atk_cancel = false;
		
		anim.SetTrigger("Free");
	}
	
	//muda pro state de ataque
	private void AnimAttack()
	{
		anim.SetTrigger("Attack");
		
		currAtk = 0;
		
		AnimHit(0);
		
		currentState = State.Attack;
	}
	
	//muda pro state de block
	private void AnimBlock()
    {
		anim.SetBool("Block", true);
		
		currentState = State.Block;
		
		P_HP.blocking = true;
		//diminui % do dano
		if(PlayerEquipment.shield_lvl < 3)
			P_HP.block_mult = (PlayerEquipment.shield_lvl < 2) ? 0.5f : 0.25f;
		//invul frames
		else
		{
			//pra não dar overwrite e diminuir os invul frames
			if(invul_f < block_f_total)
			{
				//reseta os frames de invul
				invul_f = block_f_total;
				//inicia os invul frames
				P_HP.invul = true;
			}
		}
	}
	//muda pro state de roll
	private void AnimRoll()
    {
		anim.SetBool("Roll", true);
		
		currentState = State.Roll;
		
		roll_anim_f = roll_anim_f_total;
		
		//pra não dar overwrite e diminuir os invul frames
		if(invul_f < roll_f_total)
		{
			//reseta os frames de invul
			invul_f = roll_f_total;
			//inicia os invul frames
			P_HP.invul = true;
		}
		
		//movimento
		Vector3 transf_f = transform.forward;
		Vector3 roll_direction = new Vector3(transf_f.x, 0, transf_f.z);
		
		rdb.velocity = roll_direction * rollspeed + new Vector3(0, rdb.velocity.y, 0);
    }
	
	//dano
	public void TookDamage()
	{
		currentState = State.Hurt;
		
		//pra não dar overwrite e diminuir os invul frames
		if(invul_f < hurt_f_total)
		{
			//reseta os frames de invul
			invul_f = hurt_f_total;
			//inicia os invul frames
			P_HP.invul = true;
		}

		hurt_anim_f = hurt_anim_f_total;
		
		//random range hurt_animations anim.SetInt
		anim.SetTrigger("Hurt");
	}
	public void HitBlocked()
	{
		//pra não dar overwrite e diminuir os invul frames
		if(invul_f < block_f_total)
		{
			//reseta os frames de invul
			invul_f = hurt_f_total;
			//inicia os invul frames
			P_HP.invul = true;
		}
		
		//sound effect
	}

	//player morrendo
	public void Dying()
    {
		if (currentState != State.Dead)
		{
			anim.SetTrigger("Dead");

			currentState = State.Dead;
		}
    }
	
	//acontece quando o jogador cai no chão
	private void AnimLand()
    {
		//sound effect land
    }
	#endregion

	#region inputs
	//botão de pulo pressionado
	public void JumpDown()
	{
		jumpbtn = true;
		jumpbtndown = true;
	}
	//botão de pulo solto
	public void JumpUp()
	{
		jumpbtn = false;
        jumptime = 0;
	}
	//botão de ataque pressionado
	public void AttackDown()
	{
		attackbtn = true;
		
		//buffer
		buffer_f = buffer_f_total;
		blockbtn = false;
		rollbtn = false;
	}
	//botão de bloquear pressionado
	public void BlockDown()
	{
		blockbtn = true;
		
		//buffer
		//buffer_f = buffer_f_total;
		attackbtn = false;
		rollbtn = false;
	}
		//quando solta o block
		public void BlockUp()
		{
			blockbtn = false;
		}
	//botão de roll pressionado
	public void RollDown()
	{
		rollbtn = true;
		
		//buffer
		buffer_f = buffer_f_total;
		attackbtn = false;
		blockbtn = false;
	}
	#endregion
	
	#if UNITY_EDITOR
	private void OnDrawGizmos()
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
