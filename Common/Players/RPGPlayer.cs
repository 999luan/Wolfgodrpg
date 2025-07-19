using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Skills;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using System.Linq;
using System;
using Wolfgodrpg.Common.Network;
using Terraria.DataStructures;
using Wolfgodrpg.Common.Systems;

namespace Wolfgodrpg.Common.Players
{
    /// <summary>
    /// Classe principal do jogador que gerencia todos os sistemas RPG do mod.
    /// Herda de ModPlayer para integrar com o sistema de jogadores do tModLoader.
    /// </summary>
    public class RPGPlayer : ModPlayer
    {
        // === SISTEMA DE DASH ===
        /// <summary>
        /// Cooldown restante do dash em frames.
        /// </summary>
        public int DashCooldown { get; set; } = 0;
        
        /// <summary>
        /// Número máximo de dashes disponíveis.
        /// </summary>
        public int MaxDashes { get; set; } = 3; // Aumentado para 3 dashes
        
        /// <summary>
        /// Número de dashes usados na sessão atual.
        /// </summary>
        public int DashesUsed { get; set; }
        
        /// <summary>
        /// Timer para resetar os dashes usados.
        /// </summary>
        public int DashResetTimer { get; set; }
        
        /// <summary>
        /// Velocidade do dash em pixels por frame.
        /// </summary>
        public float DashSpeed { get; set; } = 12f;
        
        /// <summary>
        /// Duração do dash em frames.
        /// </summary>
        public int DashDuration { get; set; } = 15;
        
        /// <summary>
        /// Frames de invencibilidade durante o dash.
        /// </summary>
        public int DashInvincibilityFrames { get; set; } = 15;
        
        /// <summary>
        /// Custo de stamina por dash.
        /// </summary>
        public float DashStaminaCost { get; set; } = 15f; // Reduzido para permitir mais dashes

        

        /// <summary>
        /// Lista de skills de movimentação do jogador.
        /// </summary>
        public List<Skills.BaseSkill> MovementSkills = new List<Skills.BaseSkill>();

        // === SISTEMA DE MODO DE COMBATE ===
        /// <summary>
        /// Indica se o modo de combate está ativo.
        /// </summary>
        public bool CombatModeActive { get; set; } = false;
        
        /// <summary>
        /// Cooldown do dash em ticks (usando a propriedade existente).
        /// </summary>
        
        /// <summary>
        /// Flags de desbloqueio das habilidades.
        /// </summary>
        public bool UnlockedDash { get; set; } = true;
        public bool UnlockedDoubleJump { get; set; } = false;
        public bool UnlockedWallJump { get; set; } = false;
        
        /// <summary>
        /// Estado do double jump.
        /// </summary>
        public bool usedDoubleJump = false;

        // === SISTEMAS MODULARES ===
        /// <summary>
        /// Sistema de vitais do jogador
        /// </summary>
        public VitalsSystem Vitals { get; private set; }
        
        /// <summary>
        /// Sistema de atributos do jogador
        /// </summary>
        public AttributesSystem Attributes { get; private set; }
        
        /// <summary>
        /// Sistema de subclasses do jogador
        /// </summary>
        public SubClassSystem SubClasses { get; private set; }

        // === PROPRIEDADES DE ACESSO PARA COMPATIBILIDADE ===
        /// <summary>
        /// Acesso à fome atual (para compatibilidade)
        /// </summary>
        public float CurrentHunger
        {
            get => Vitals?.CurrentHunger ?? 100f;
            set { if (Vitals != null) Vitals.CurrentHunger = value; }
        }

        /// <summary>
        /// Acesso à sanidade atual (para compatibilidade)
        /// </summary>
        public float CurrentSanity
        {
            get => Vitals?.CurrentSanity ?? 100f;
            set { if (Vitals != null) Vitals.CurrentSanity = value; }
        }

        /// <summary>
        /// Acesso à stamina atual (para compatibilidade)
        /// </summary>
        public float CurrentStamina
        {
            get => Vitals?.CurrentStamina ?? 100f;
            set { if (Vitals != null) Vitals.CurrentStamina = value; }
        }

        /// <summary>
        /// Acesso à stamina máxima (para compatibilidade)
        /// </summary>
        public float MaxStamina => VitalsSystem.MAX_STAMINA;

        // === ATRIBUTOS PRIMÁRIOS === ⭐ NOVO
        /// <summary>
        /// Força do jogador. Afeta dano corpo a corpo e capacidade de carga.
        /// </summary>
        public int Strength { get; set; } = 10;
        
        /// <summary>
        /// Destreza do jogador. Afeta dano à distância, chance crítica e velocidade de ataque.
        /// </summary>
        public int Dexterity { get; set; } = 10;
        
        /// <summary>
        /// Inteligência do jogador. Afeta dano mágico, mana máxima e velocidade de conjuração.
        /// </summary>
        public int Intelligence { get; set; } = 10;
        
        /// <summary>
        /// Constituição do jogador. Afeta vida máxima, defesa e regeneração de vida.
        /// </summary>
        public int Constitution { get; set; } = 10;
        
        /// <summary>
        /// Sabedoria do jogador. Afeta dano de invocação, sorte e resistência a debuffs.
        /// </summary>
        public int Wisdom { get; set; } = 10;

        // === NÍVEL DO JOGADOR === ⭐ NOVO
        /// <summary>
        /// Nível geral do jogador.
        /// </summary>
        public int PlayerLevel { get; set; } = 1;
        
        /// <summary>
        /// Experiência geral do jogador.
        /// </summary>
        public float PlayerExperience { get; set; } = 0f;
        
        /// <summary>
        /// Pontos de atributo disponíveis para distribuição.
        /// </summary>
        public int AttributePoints { get; set; } = 0;

        // === SISTEMA DE PROFICIÊNCIA DE ARMADURAS === ⭐ NOVO
        /// <summary>
        /// Níveis de proficiência para cada tipo de armadura.
        /// </summary>
        public Dictionary<ArmorType, int> ArmorProficiencyLevels = new Dictionary<ArmorType, int>();
        
        /// <summary>
        /// Experiência atual de proficiência para cada tipo de armadura.
        /// </summary>
        public Dictionary<ArmorType, float> ArmorProficiencyExperience = new Dictionary<ArmorType, float>();

        // === SISTEMA DE PROFICIÊNCIA DE ARMAS === ⭐ NOVO
        /// <summary>
        /// Níveis de proficiência para cada tipo de arma.
        /// </summary>
        public Dictionary<WeaponType, int> WeaponProficiencyLevels = new Dictionary<WeaponType, int>();
        
        /// <summary>
        /// Experiência atual de proficiência para cada tipo de arma.
        /// </summary>
        public Dictionary<WeaponType, float> WeaponProficiencyExperience = new Dictionary<WeaponType, float>();

        // === SISTEMA DE DASH SOULS-LIKE ===
        /// <summary>
        /// Timers individuais para cada direção do dash
        /// </summary>
        private int leftTapTimer = 0;
        private int rightTapTimer = 0;
        private int upTapTimer = 0;
        private int downTapTimer = 0;
        
        /// <summary>
        /// Tempo máximo para o duplo toque (15 ticks)
        /// </summary>
        private const int DOUBLE_TAP_TIME = 15;
        
        /// <summary>
        /// Velocidade do dash
        /// </summary>
        private const float DASH_SPEED = 12f;
        
        /// <summary>
        /// Duração do dash em ticks
        /// </summary>
        private const int DASH_DURATION = 10;
        
        /// <summary>
        /// Timer do dash atual
        /// </summary>
        private int dashTimer = 0;
        
        /// <summary>
        /// Direção do dash atual
        /// </summary>
        private Vector2 dashDirection = Vector2.Zero;
        
        /// <summary>
        /// Flag indicando se está fazendo dash
        /// </summary>
        private bool isDashing = false;
        
        /// <summary>
        /// Porcentagem de stamina consumida por dash
        /// </summary>
        private const float DASH_STAMINA_COST_PERCENT = 10f;
        
        /// <summary>
        /// Timer de stun quando stamina chega a zero (2 segundos = 120 frames)
        /// </summary>
        private int stunTimer = 0;
        
        /// <summary>
        /// Flag indicando se está stunado
        /// </summary>
        private bool isStunned = false;

        // Flag para autodash (será ativada pelo item)
        public bool AutoDashEnabled = false;
        private const int DoubleTapTime = 15;

        // Variáveis para dash direcional (REMOVIDAS - usando novo sistema Souls-like)
        // private int dashTimer = 0;
        // private Vector2 dashDirection = Vector2.Zero;
        // private bool isDashing = false; // Flag para indicar se está fazendo dash

        /// <summary>
        /// Função de easing cúbico para animações suaves.
        /// </summary>
        /// <param name="t">Progresso da animação (0-1)</param>
        /// <returns>Valor suavizado</returns>
        private float EaseInOutCubic(float t)
        {
            return t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3f) / 2f;
        }

        // === LOGS DE XP ACUMULADOS ===
        public List<string> XPLogs = new List<string>();

        public void AddXPLog(string log)
        {
            XPLogs.Add(log);
        }

        public void ClearXPLogs()
        {
            XPLogs.Clear();
        }

        /// <summary>
        /// Inicializa o jogador com valores padrão.
        /// </summary>
        public override void Initialize()
        {
            // Inicializar sistemas modulares
            Vitals = new VitalsSystem();
            Attributes = new AttributesSystem();
            SubClasses = new SubClassSystem();
            
            // A inicialização de classes agora é feita em SubClassSystem
            // foreach (var className in RPGClassDefinitions.ActionClasses.Keys)
            // {
            //     ClassLevels[className] = 0f;
            //     ClassExperience[className] = 0f;
            // }
            
            // Inicializar proficiências de armadura ⭐ NOVO
            foreach (ArmorType armorType in System.Enum.GetValues<ArmorType>())
            {
                ArmorProficiencyLevels[armorType] = 1;
                ArmorProficiencyExperience[armorType] = 0f;
            }

            // Inicializar proficiências de arma ⭐ NOVO
            foreach (WeaponType weaponType in System.Enum.GetValues<WeaponType>())
            {
                WeaponProficiencyLevels[weaponType] = 1;
                WeaponProficiencyExperience[weaponType] = 0f;
            }
            
            // Inicializar atributos primários (usando o sistema modular)
            Strength = Attributes.Strength;
            Dexterity = Attributes.Dexterity;
            Intelligence = Attributes.Intelligence;
            Constitution = Attributes.Constitution;
            Wisdom = Attributes.Wisdom;

            // Inicializar nível do jogador
            PlayerLevel = 1;
            PlayerExperience = 0f;
            AttributePoints = 0;
            
            // Resetar dash
            DashCooldown = 0;
            DashesUsed = 0;
            DashResetTimer = 0;
            
            // Inicializar skills de movimentação
            InitializeMovementSkills();
        }

        /// <summary>
        /// Atualiza o jogador a cada frame.
        /// </summary>
        public override void PostUpdate()
        {
            UpdateVitals();
            UpdateDash();
            UpdateStunEffects();
            ProcessMilestoneEffects();
            UpdateMovementSkills();
            
            // Atualizar skills das subclasses
            SubClasses?.UpdateAllSkills();
        }
        
        /// <summary>
        /// Atualiza efeitos visuais do stun.
        /// </summary>
        private void UpdateStunEffects()
        {
            if (isStunned && stunTimer > 0)
            {
                // Efeitos visuais durante o stun
                if (Main.rand.NextBool(3)) // 33% de chance por frame
                {
                    // Partículas de exaustão
                    Dust.NewDustDirect(
                        Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                        0, 0, DustID.Smoke,
                        Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f),
                        100, Color.Gray, 0.8f
                    );
                }
                
                // Efeito de tela tremendo
                if (Main.rand.NextBool(10)) // 10% de chance por frame
                {
                    // Efeito visual de stun (partículas extras)
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustDirect(
                            Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                            0, 0, DustID.Smoke,
                            Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f),
                            80, Color.DarkGray, 1.2f
                        );
                    }
                }
                
                // Feedback visual do tempo restante
                if (stunTimer % 60 == 0) // A cada segundo
                {
                    float remainingSeconds = stunTimer / 60f;
                    Main.NewText($"Stunned! {remainingSeconds:F1}s remaining...", Color.Red);
                }
            }
        }

        /// <summary>
        /// Atualiza antes do movimento do jogador.
        /// </summary>
        public override void PreUpdateMovement()
        {
            // Resetar double jump quando tocar o chão
            if (Player.velocity.Y == 0)
            {
                usedDoubleJump = false;
            }

            // Atualizar cooldown do dash
            if (DashCooldown > 0)
                DashCooldown--;

            // === DOUBLE JUMP ===
            if (CombatModeActive && UnlockedDoubleJump && Player.controlJump && !usedDoubleJump && !Player.velocity.Y.Equals(0))
            {
                if (ConsumeStaminaPercent(10f))
                {
                    Player.velocity.Y = -Player.jumpSpeed * 0.8f;
                    usedDoubleJump = true;
                    
                    // Efeito visual
                    for (int i = 0; i < 5; i++)
                    {
                        Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Cloud,
                            Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 0, Color.White);
                    }
                    
                    // Efeito sonoro
                    SoundEngine.PlaySound(SoundID.Item24, Player.position);
                }
            }

            // === WALL JUMP ===
            if (CombatModeActive && UnlockedWallJump && Player.controlJump && Player.velocity.Y > 0)
            {
                if (IsTouchingWall(out int side))
                {
                    if (ConsumeStaminaPercent(10f))
                    {
                        Player.velocity.Y = -Player.jumpSpeed * 0.9f;
                        Player.velocity.X = 6f * -side;
                        
                        // Efeito visual
                        for (int i = 0; i < 8; i++)
                        {
                            Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Stone,
                                Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 0, Color.Gray);
                        }
                        
                        // Efeito sonoro
                        SoundEngine.PlaySound(SoundID.Item24, Player.position);
                    }
                }
            }
        }

        /// <summary>
        /// Processa triggers de input para o sistema de dash Souls-like.
        /// </summary>
        /// <param name="triggersSet">Conjunto de triggers ativos</param>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // === SISTEMA DE DASH SOULS-LIKE COM DUPLO TOQUE ===
            if (CombatModeActive && UnlockedDash && !isDashing && !isStunned)
            {
                // Verificar se tem stamina suficiente para dash
                if (CurrentStamina < DASH_STAMINA_COST_PERCENT)
                {
                    // Feedback visual quando não tem stamina
                    if (Main.rand.NextBool(60)) // A cada segundo
                    {
                        Main.NewText("Not enough stamina for dash!", Color.Orange);
                    }
                    return; // Não permite dash sem stamina
                }
                
                // === DASH PARA ESQUERDA (A) ===
                if (Player.controlLeft)
                {
                    if (leftTapTimer == 0)
                    {
                        leftTapTimer = DOUBLE_TAP_TIME;
                    }
                    else if (leftTapTimer > 0 && leftTapTimer < DOUBLE_TAP_TIME)
                    {
                        // Duplo toque detectado - executar dash
                        if (ConsumeStaminaPercent(DASH_STAMINA_COST_PERCENT))
                        {
                            ExecuteDash(new Vector2(-1f, 0f));
                            leftTapTimer = 0;
                        }
                        else
                        {
                            // Feedback visual quando não tem stamina
                            Main.NewText("Not enough stamina!", Color.Orange);
                            leftTapTimer = 0;
                        }
                    }
                }
                else
                {
                    if (leftTapTimer > 0) leftTapTimer--;
                }

                // === DASH PARA DIREITA (D) ===
                if (Player.controlRight)
                {
                    if (rightTapTimer == 0)
                    {
                        rightTapTimer = DOUBLE_TAP_TIME;
                    }
                    else if (rightTapTimer > 0 && rightTapTimer < DOUBLE_TAP_TIME)
                    {
                        // Duplo toque detectado - executar dash
                        if (ConsumeStaminaPercent(DASH_STAMINA_COST_PERCENT))
                        {
                            ExecuteDash(new Vector2(1f, 0f));
                            rightTapTimer = 0;
                        }
                        else
                        {
                            // Feedback visual quando não tem stamina
                            Main.NewText("Not enough stamina!", Color.Orange);
                            rightTapTimer = 0;
                        }
                    }
                }
                else
                {
                    if (rightTapTimer > 0) rightTapTimer--;
                }

                // === DASH PARA CIMA (W) ===
                if (Player.controlUp)
                {
                    if (upTapTimer == 0)
                    {
                        upTapTimer = DOUBLE_TAP_TIME;
                    }
                    else if (upTapTimer > 0 && upTapTimer < DOUBLE_TAP_TIME)
                    {
                        // Duplo toque detectado - executar dash
                        if (ConsumeStaminaPercent(DASH_STAMINA_COST_PERCENT))
                        {
                            ExecuteDash(new Vector2(0f, -1f));
                            upTapTimer = 0;
                        }
                        else
                        {
                            // Feedback visual quando não tem stamina
                            Main.NewText("Not enough stamina!", Color.Orange);
                            upTapTimer = 0;
                        }
                    }
                }
                else
                {
                    if (upTapTimer > 0) upTapTimer--;
                }

                // === DASH PARA BAIXO (S) ===
                if (Player.controlDown)
                {
                    if (downTapTimer == 0)
                    {
                        downTapTimer = DOUBLE_TAP_TIME;
                    }
                    else if (downTapTimer > 0 && downTapTimer < DOUBLE_TAP_TIME)
                    {
                        // Duplo toque detectado - executar dash
                        if (ConsumeStaminaPercent(DASH_STAMINA_COST_PERCENT))
                        {
                            ExecuteDash(new Vector2(0f, 1f));
                            downTapTimer = 0;
                        }
                        else
                        {
                            // Feedback visual quando não tem stamina
                            Main.NewText("Not enough stamina!", Color.Orange);
                            downTapTimer = 0;
                        }
                    }
                }
                else
                {
                    if (downTapTimer > 0) downTapTimer--;
                }
            }
        }

        /// <summary>
        /// Método público para ganhar XP de proficiência de armadura quando o jogador recebe dano.
        /// </summary>
        /// <param name="damage">Quantidade de dano recebido</param>
        public void OnPlayerDamaged(int damage)
        {
            Mod.Logger.Debug($"[WolfgodRPG] OnPlayerDamaged called. Damage: {damage}");
            if (damage > 0)
            {
                ArmorType currentArmorType = GetEquippedArmorType();
                Mod.Logger.Debug($"[WolfgodRPG] Detected ArmorType: {currentArmorType}");
                if (currentArmorType != ArmorType.None)
                {
                    float xpGained = damage * 0.1f;
                    GainArmorProficiencyXP(currentArmorType, xpGained);
                    Mod.Logger.Debug($"[WolfgodRPG] Gained {xpGained} XP for {currentArmorType} proficiency. Current XP: {ArmorProficiencyExperience[currentArmorType]}");
                }
                else
                {
                    Mod.Logger.Debug($"[WolfgodRPG] No ArmorType detected for equipped armor.");
                }
            }
        }

        /// <summary>
        /// Chamado quando o jogador atinge um NPC.
        /// </summary>
        /// <param name="item">Item usado para atingir o NPC</param>
        /// <param name="target">NPC atingido</param>
        /// <param name="hit">Informações do hit</param>
        /// <param name="damage">Dano causado</param>
        public void OnHitNPC(Item item, NPC target, NPC.HitInfo hit, int damage)
        {
            Mod.Logger.Debug($"[WolfgodRPG] OnHitNPC called. Item: {item.Name}, Damage: {damage}");
            if (damage > 0)
            {
                WeaponType currentWeaponType = GetWeaponType(item);
                Mod.Logger.Debug($"[WolfgodRPG] Detected WeaponType: {currentWeaponType}");
                if (currentWeaponType != WeaponType.None)
                {
                    float xpGained = damage * 0.05f;
                    AddWeaponProficiencyXP(currentWeaponType, xpGained); // Ganha XP baseado no dano
                    Mod.Logger.Debug($"[WolfgodRPG] Gained {xpGained} XP for {currentWeaponType} proficiency. Current XP: {WeaponProficiencyExperience[currentWeaponType]}");
                }
                else
                {
                    Mod.Logger.Debug($"[WolfgodRPG] No WeaponType detected for item: {item.Name}");
                }
            }
        }

        /// <summary>
        /// Determina o tipo de arma do item.
        /// </summary>
        /// <param name="item">Item a ser verificado</param>
        /// <returns>Tipo de arma</returns>
        public WeaponType GetWeaponType(Item item)
        {
            // Verificar por DamageType primeiro (mais confiável)
            if (item.DamageType == DamageClass.Melee)
                return WeaponType.Melee;
            if (item.DamageType == DamageClass.Ranged)
                return WeaponType.Ranged;
            if (item.DamageType == DamageClass.Magic)
                return WeaponType.Magic;
            if (item.DamageType == DamageClass.Summon)
                return WeaponType.Summon;
            
            // Fallback: verificar por nome do item
            string itemName = item.Name.ToLower();
            
            // Melee weapons
            if (itemName.Contains("sword") || itemName.Contains("axe") || itemName.Contains("hammer") || 
                itemName.Contains("spear") || itemName.Contains("lance") || itemName.Contains("dagger") ||
                itemName.Contains("knife") || itemName.Contains("mace") || itemName.Contains("flail") ||
                itemName.Contains("broadsword") || itemName.Contains("shortsword") || itemName.Contains("katana") ||
                itemName.Contains("rapier") || itemName.Contains("saber") || itemName.Contains("cutlass") ||
                itemName.Contains("claymore") || itemName.Contains("greatsword") || itemName.Contains("warhammer"))
                return WeaponType.Melee;
            
            // Ranged weapons
            if (itemName.Contains("bow") || itemName.Contains("gun") || itemName.Contains("rifle") ||
                itemName.Contains("pistol") || itemName.Contains("revolver") || itemName.Contains("crossbow") ||
                itemName.Contains("blowgun") || itemName.Contains("dart") || itemName.Contains("arrow") ||
                itemName.Contains("musket") || itemName.Contains("shotgun") || itemName.Contains("sniper") ||
                itemName.Contains("repeater") || itemName.Contains("harpoon") || itemName.Contains("javelin"))
                return WeaponType.Ranged;
            
            // Magic weapons
            if (itemName.Contains("staff") || itemName.Contains("wand") || itemName.Contains("book") ||
                itemName.Contains("spell") || itemName.Contains("magic") || itemName.Contains("crystal") ||
                itemName.Contains("orb") || itemName.Contains("tome") || itemName.Contains("grimoire") ||
                itemName.Contains("scroll") || itemName.Contains("scepter") || itemName.Contains("rod") ||
                itemName.Contains("charm") || itemName.Contains("amulet") || itemName.Contains("ring"))
                return WeaponType.Magic;
            
            // Summon weapons
            if (itemName.Contains("whip") || itemName.Contains("summon") || itemName.Contains("staff") ||
                itemName.Contains("rod") || itemName.Contains("crystal") || itemName.Contains("minion") ||
                itemName.Contains("sentinel") || itemName.Contains("familiar") || itemName.Contains("totem") ||
                itemName.Contains("idol") || itemName.Contains("effigy") || itemName.Contains("doll"))
                return WeaponType.Summon;
            
            return WeaponType.None;
        }

        /// <summary>
        /// Salva os dados do jogador usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound para salvar os dados</param>
        public override void SaveData(TagCompound tag)
        {
            SubClasses?.SaveData(tag);
            // A serialização de classes foi movida para SubClassSystem
            // Salvar vitals
            tag["CurrentHunger"] = CurrentHunger;
            tag["CurrentSanity"] = CurrentSanity;
            tag["CurrentStamina"] = CurrentStamina;
            // Salvar dados de dash
            tag["DashCooldown"] = DashCooldown;
            tag["DashesUsed"] = DashesUsed;
            tag["DashResetTimer"] = DashResetTimer;
            tag["MaxDashes"] = MaxDashes;

            // Salvar atributos primários
            tag["Strength"] = Strength;
            tag["Dexterity"] = Dexterity;
            tag["Intelligence"] = Intelligence;
            tag["Constitution"] = Constitution;
            tag["Wisdom"] = Wisdom;

            // Salvar nível do jogador
            tag["PlayerLevel"] = PlayerLevel;
            tag["PlayerExperience"] = PlayerExperience;
            tag["AttributePoints"] = AttributePoints;
            
            // Salvar proficiências de armadura ⭐ NOVO
            var levelsList = new List<TagCompound>();
            foreach (var kvp in ArmorProficiencyLevels)
            {
                levelsList.Add(new TagCompound
                {
                    ["Key"] = kvp.Key.ToString(),
                    ["Value"] = kvp.Value
                });
            }
            tag["ArmorProficiencyLevels"] = levelsList;
            
            var experienceList = new List<TagCompound>();
            foreach (var kvp in ArmorProficiencyExperience)
            {
                experienceList.Add(new TagCompound
                {
                    ["Key"] = kvp.Key.ToString(),
                    ["Value"] = kvp.Value
                });
            }
            tag["ArmorProficiencyExperience"] = experienceList;

            // Salvar proficiências de arma ⭐ NOVO
            var weaponLevelsList = new List<TagCompound>();
            foreach (var kvp in WeaponProficiencyLevels)
            {
                weaponLevelsList.Add(new TagCompound
                {
                    ["Key"] = kvp.Key.ToString(),
                    ["Value"] = kvp.Value
                });
            }
            tag["WeaponProficiencyLevels"] = weaponLevelsList;

            var weaponExperienceList = new List<TagCompound>();
            foreach (var kvp in WeaponProficiencyExperience)
            {
                weaponExperienceList.Add(new TagCompound
                {
                    ["Key"] = kvp.Key.ToString(),
                    ["Value"] = kvp.Value
                });
            }
            tag["WeaponProficiencyExperience"] = weaponExperienceList;
        }

        /// <summary>
        /// Carrega os dados do jogador usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound contendo os dados salvos</param>
        public override void LoadData(TagCompound tag)
        {
            SubClasses?.LoadData(tag);
            // A desserialização de classes foi movida para SubClassSystem
            // Carregar vitals
            if (tag.ContainsKey("CurrentHunger"))
                CurrentHunger = tag.GetFloat("CurrentHunger");
            if (tag.ContainsKey("CurrentSanity"))
                CurrentSanity = tag.GetFloat("CurrentSanity");
            if (tag.ContainsKey("CurrentStamina"))
                CurrentStamina = tag.GetFloat("CurrentStamina");
            // Carregar dados de dash
            if (tag.ContainsKey("DashCooldown"))
                DashCooldown = tag.GetInt("DashCooldown");
            if (tag.ContainsKey("DashesUsed"))
                DashesUsed = tag.GetInt("DashesUsed");
            if (tag.ContainsKey("DashResetTimer"))
                DashResetTimer = tag.GetInt("DashResetTimer");
            if (tag.ContainsKey("MaxDashes"))
                MaxDashes = tag.GetInt("MaxDashes");

            // Carregar atributos primários
            if (tag.ContainsKey("Strength"))
                Strength = tag.GetInt("Strength");
            if (tag.ContainsKey("Dexterity"))
                Dexterity = tag.GetInt("Dexterity");
            if (tag.ContainsKey("Intelligence"))
                Intelligence = tag.GetInt("Intelligence");
            if (tag.ContainsKey("Constitution"))
                Constitution = tag.GetInt("Constitution");
            if (tag.ContainsKey("Wisdom"))
                Wisdom = tag.GetInt("Wisdom");

            // Carregar nível do jogador
            if (tag.ContainsKey("PlayerLevel"))
                PlayerLevel = tag.GetInt("PlayerLevel");
            if (tag.ContainsKey("PlayerExperience"))
                PlayerExperience = tag.GetFloat("PlayerExperience");
            if (tag.ContainsKey("AttributePoints"))
                AttributePoints = tag.GetInt("AttributePoints");
            
            // Carregar proficiências de armadura ⭐ NOVO
            if (tag.ContainsKey("ArmorProficiencyLevels"))
            {
                var levels = tag.GetList<TagCompound>("ArmorProficiencyLevels");
                foreach (var levelTag in levels)
                {
                    if (levelTag.ContainsKey("Key") && levelTag.ContainsKey("Value"))
                    {
                        string key = levelTag.GetString("Key");
                        int value = levelTag.GetInt("Value");
                        if (System.Enum.TryParse<ArmorType>(key, out ArmorType type))
                        {
                            ArmorProficiencyLevels[type] = value;
                        }
                    }
                }
            }
            
            if (tag.ContainsKey("ArmorProficiencyExperience"))
            {
                var experience = tag.GetList<TagCompound>("ArmorProficiencyExperience");
                foreach (var expTag in experience)
                {
                    if (expTag.ContainsKey("Key") && expTag.ContainsKey("Value"))
                    {
                        string key = expTag.GetString("Key");
                        float value = expTag.GetFloat("Value");
                        if (System.Enum.TryParse<ArmorType>(key, out ArmorType type))
                        {
                            ArmorProficiencyExperience[type] = value;
                        }
                    }
                }
            }

            // Carregar proficiências de arma ⭐ NOVO
            if (tag.ContainsKey("WeaponProficiencyLevels"))
            {
                var levels = tag.GetList<TagCompound>("WeaponProficiencyLevels");
                foreach (var levelTag in levels)
                {
                    if (levelTag.ContainsKey("Key") && levelTag.ContainsKey("Value"))
                    {
                        string key = levelTag.GetString("Key");
                        int value = levelTag.GetInt("Value");
                        if (System.Enum.TryParse<WeaponType>(key, out WeaponType type))
                        {
                            WeaponProficiencyLevels[type] = value;
                        }
                    }
                }
            }

            if (tag.ContainsKey("WeaponProficiencyExperience"))
            {
                var experience = tag.GetList<TagCompound>("WeaponProficiencyExperience");
                foreach (var expTag in experience)
                {
                    if (expTag.ContainsKey("Key") && expTag.ContainsKey("Value"))
                    {
                        string key = expTag.GetString("Key");
                        float value = expTag.GetFloat("Value");
                        if (System.Enum.TryParse<WeaponType>(key, out WeaponType type))
                        {
                            WeaponProficiencyExperience[type] = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sincroniza dados do jogador com outros clientes.
        /// </summary>
        /// <param name="toWho">ID do jogador que receberá os dados</param>
        /// <param name="fromWho">ID do jogador que enviou os dados</param>
        /// <param name="newPlayer">Se é um novo jogador</param>
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            var packet = Mod.GetPacket();
            packet.Write((byte)WolfgodrpgMessageType.SyncRPGPlayer);
            packet.Write((byte)Player.whoAmI);
            
            // Enviar vitals
            packet.Write(CurrentHunger);
            packet.Write(CurrentSanity);
            packet.Write(CurrentStamina);
            
            // Class data is now handled by SubClasses.SaveData() and SubClasses.LoadData()
        }

        /// <summary>
        /// Envia mudanças do cliente para o servidor.
        /// </summary>
        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            var clientRPGPlayer = clientPlayer as RPGPlayer;
            if (clientRPGPlayer == null)
                return;

            // Enviar apenas se houve mudanças significativas
            if (HasSignificantChanges(clientRPGPlayer))
            {
                var packet = Mod.GetPacket();
                packet.Write((byte)WolfgodrpgMessageType.SyncRPGPlayer);
                packet.Write((byte)Player.whoAmI);
                
                // Enviar dados atualizados
                packet.Write(CurrentHunger);
                packet.Write(CurrentSanity);
                packet.Write(CurrentStamina);
                
                packet.Send();
            }
        }

        /// <summary>
        /// Copia o estado do servidor para o cliente.
        /// </summary>
        /// <param name="clientPlayer">Jogador do cliente</param>
        public override void CopyClientState(ModPlayer clientPlayer)
        {
            var clientRPGPlayer = clientPlayer as RPGPlayer;
            if (clientRPGPlayer == null)
                return;

            // Copiar vitals
            clientRPGPlayer.CurrentHunger = CurrentHunger;
            clientRPGPlayer.CurrentSanity = CurrentSanity;
            clientRPGPlayer.CurrentStamina = CurrentStamina;
            
            // Class data is now handled by SubClasses.LoadData()
        }

        /// <summary>
        /// Verifica se houve mudanças significativas que precisam ser sincronizadas.
        /// </summary>
        /// <param name="clientPlayer">Jogador do cliente para comparação</param>
        /// <returns>True se houve mudanças significativas</returns>
        private bool HasSignificantChanges(RPGPlayer clientPlayer)
        {
            // Verificar mudanças nos vitals
            bool vitalsChanged = false;
            if (clientPlayer != null)
            {
                vitalsChanged = Math.Abs(CurrentHunger - clientPlayer.CurrentHunger) > 0.1f ||
                               Math.Abs(CurrentSanity - clientPlayer.CurrentSanity) > 0.1f ||
                               Math.Abs(CurrentStamina - clientPlayer.CurrentStamina) > 0.1f;
            }

            // Class data changes are now handled by SubClasses system
            bool classDataChanged = false; // TODO: Implementar verificação de mudanças nas subclasses se necessário

            return vitalsChanged || classDataChanged;
        }

        /// <summary>
        /// Atualiza o sistema de vitais do jogador.
        /// </summary>
        private void UpdateVitals()
        {
            if (Vitals == null) return;
            
            // === SISTEMA DE FOME ===
            // Fome diminui 1% por minuto (60 segundos = 3600 frames)
            if (Main.GameUpdateCount % 3600 == 0) // A cada minuto
            {
                CurrentHunger = Math.Max(0f, CurrentHunger - 1f);
            }
            
            // === SISTEMA DE STAMINA ===
            // Stamina regenera de 2 formas:
            // 1. Naturalmente e rapidamente fora do modo de combate
            // 2. Automaticamente quando gasta 100% e diminui a fome (com stun de 2 segundos)
            if (!CombatModeActive)
            {
                // Regeneração rápida fora do modo de combate
                CurrentStamina = Math.Min(100f, CurrentStamina + 1f); // Regeneração mais rápida
            }
            else
            {
                // No modo de combate, só regenera se gastou 100% e diminuiu a fome
                if (CurrentStamina <= 0f && !isStunned)
                {
                    // Ativar stun por 2 segundos
                    stunTimer = 120; // 2 segundos = 120 frames
                    isStunned = true;
                    
                    // Feedback visual do stun
                    Main.NewText("You are exhausted! Stunned for 2 seconds!", Color.Red);
                    SoundEngine.PlaySound(SoundID.Item25, Player.position);
                    
                    // Aplicar efeito visual de stun
                    Player.AddBuff(BuffID.Slow, 120); // Slow por 2 segundos
                }
                
                // Atualizar timer de stun
                if (stunTimer > 0)
                {
                    stunTimer--;
                    if (stunTimer == 0)
                    {
                        // Stun acabou, regenerar stamina automaticamente
                        isStunned = false;
                        
                        // Consumir 1 de fome para regenerar stamina automaticamente
                        if (CurrentHunger > 0f)
                        {
                            CurrentHunger = Math.Max(0f, CurrentHunger - 1f);
                            CurrentStamina = 100f; // Regenera automaticamente para 100%
                            
                            // Feedback visual
                            Main.NewText("Stamina restored! Hunger decreased.", Color.Yellow);
                            SoundEngine.PlaySound(SoundID.Item37, Player.position);
                        }
                        else
                        {
                            // Se não tem fome, regenera só 50%
                            CurrentStamina = 50f;
                            Main.NewText("Stamina partially restored (no hunger).", Color.Orange);
                        }
                    }
                }
            }
            
            // === EFEITOS DA FOME ===
            // Fome abaixo de 50%: perde 50% de dano e velocidade
            if (CurrentHunger < 50f)
            {
                Player.GetDamage(DamageClass.Generic) *= 0.5f;
                Player.moveSpeed *= 0.5f;
            }
            // Fome entre 70% e 100%: ganha 50% de dano
            else if (CurrentHunger >= 70f && CurrentHunger <= 100f)
            {
                Player.GetDamage(DamageClass.Generic) *= 1.5f;
            }
            
            // Fome no zero: começa a perder vida
            if (CurrentHunger <= 0f)
            {
                Player.statLife = Math.Max(1, Player.statLife - 1);
                if (Main.rand.NextBool(60)) // A cada segundo (60 FPS)
                {
                    Main.NewText("You are starving!", Color.Red);
                }
            }
            
            // === SISTEMA DE SANIDADE ===
            // Sanidade diminui durante combate ou no escuro
            bool isInCombat = CombatModeActive;
            bool isInDarkness = !Main.dayTime || Player.ZoneRockLayerHeight || Player.ZoneUnderworldHeight;
            
            if (isInCombat || isInDarkness)
            {
                // Diminui 5% por minuto (5% / 60 segundos = 0.083% por segundo)
                // 0.083% / 60 frames = 0.00138% por frame
                CurrentSanity = Math.Max(0f, CurrentSanity - 0.00138f);
            }
            else
            {
                // Regenera dentro de casas/bases
                bool isInHouse = IsPlayerInHouse();
                if (isInHouse)
                {
                    // Regenera 100% em 5 minutos = 20% por minuto = 0.33% por segundo = 0.0055% por frame
                    CurrentSanity = Math.Min(100f, CurrentSanity + 0.0055f);
                }
            }
            
            // === EFEITOS DA SANIDADE ===
            // Sanidade zero: efeito de confusão
            if (CurrentSanity <= 0f)
            {
                Player.AddBuff(BuffID.Confused, 60); // 1 segundo de confusão
                if (Main.rand.NextBool(120)) // A cada 2 segundos
                {
                    Main.NewText("Your mind is confused!", Color.Purple);
                }
            }
            
            // Aplicar efeitos baseados nos vitals
            ApplyVitalEffects();
        }
        
        /// <summary>
        /// Verifica se o jogador está dentro de uma casa ou base.
        /// </summary>
        /// <returns>True se está em uma casa</returns>
        private bool IsPlayerInHouse()
        {
            // Verificar se está próximo a mobiliário ou paredes de casa
            int houseX = (int)(Player.position.X / 16f);
            int houseY = (int)(Player.position.Y / 16f);
            
            // Verificar em uma área de 10x10 tiles ao redor do jogador
            for (int x = houseX - 5; x <= houseX + 5; x++)
            {
                for (int y = houseY - 5; y <= houseY + 5; y++)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {
                        Tile tile = Main.tile[x, y];
                        
                        // Verificar se há paredes de casa
                        if (tile.WallType == WallID.Wood || 
                            tile.WallType == WallID.Stone || 
                            tile.WallType == WallID.Glass)
                        {
                            return true;
                        }
                        
                        // Verificar se há mobiliário
                        if (tile.TileType == TileID.Chairs || 
                            tile.TileType == TileID.Tables ||
                            tile.TileType == TileID.Beds ||
                            tile.TileType == TileID.Bookcases)
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Chamado quando um NPC morre.
        /// </summary>
        /// <param name="npc">NPC que morreu</param>
        public void OnNPCDied(NPC npc)
        {
            // Chance de dropar comida (15% para todos os inimigos)
            if (Main.rand.NextFloat() < 0.15f)
            {
                DropFoodFromNPC(npc);
            }
        }
        
        /// <summary>
        /// Faz um NPC dropar comida.
        /// </summary>
        /// <param name="npc">NPC que vai dropar a comida</param>
        private void DropFoodFromNPC(NPC npc)
        {
            // Lista de itens de comida básicos que existem no Terraria
            int[] foodItems = {
                ItemID.Apple, ItemID.Banana, ItemID.Lemon,
                ItemID.Pineapple, ItemID.Coconut, ItemID.Peach,
                ItemID.Plum, ItemID.Cherry, ItemID.Apricot,
                ItemID.Mango
            };
            
            // Escolher um item aleatório
            int selectedFood = foodItems[Main.rand.Next(foodItems.Length)];
            
            // Dropar o item
            Item.NewItem(npc.GetSource_Death(), npc.position, npc.width, npc.height, selectedFood, 1);
            
            // Feedback visual
            Main.NewText($"{npc.FullName} dropped food!", Color.Green);
        }

        /// <summary>
        /// Aplica efeitos baseados no estado dos vitals.
        /// </summary>
        private void ApplyVitalEffects()
        {
            // Low hunger effects
            if (CurrentHunger < 20f)
            {
                Player.moveSpeed *= 0.8f;
                Player.jumpSpeedBoost *= 0.8f;
            }
            
            // Low sanity effects
            if (CurrentSanity < 30f)
            {
                Player.statDefense -= 5;
            }
            
            // Low stamina effects
            if (CurrentStamina < 25f)
            {
                Player.moveSpeed *= 0.9f;
            }
        }

        /// <summary>
        /// Atualiza o sistema de dash Souls-like.
        /// </summary>
        private void UpdateDash()
        {
            if (dashTimer > 0)
            {
                // Calcular progresso da animação (0-1)
                float progress = 1f - (dashTimer / (float)DASH_DURATION);
                
                // Criar uma animação de rolagem suave usando função de easing
                float easedProgress = EaseInOutCubic(progress);
                
                // Calcular rotação baseada no progresso (360 graus)
                float rotationAngle = easedProgress * MathHelper.TwoPi;
                
                // Aplicar rotação baseada na direção do dash
                if (dashDirection.X < 0) // Dash para esquerda
                {
                    Player.fullRotation = -rotationAngle;
                }
                else if (dashDirection.X > 0) // Dash para direita
                {
                    Player.fullRotation = rotationAngle;
                }
                else if (dashDirection.Y < 0) // Dash para cima
                {
                    Player.fullRotation = rotationAngle;
                }
                else if (dashDirection.Y > 0) // Dash para baixo
                {
                    Player.fullRotation = -rotationAngle;
                }
                
                // Adicionar efeitos visuais durante o dash
                if (Main.rand.NextBool(2)) // 50% de chance por frame
                {
                    // Criar partículas de velocidade
                    Dust.NewDustDirect(
                        Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                        0, 0, DustID.Smoke, 
                        -dashDirection.X * 1.5f, -dashDirection.Y * 1.5f, 
                        80, Color.White, 0.6f
                    );
                }
                
                dashTimer--;
                if (dashTimer == 0)
                {
                    // Finalizar animação
                    Player.fullRotation = 0f;
                    isDashing = false;
                }
            }
        }

        // Método PerformDash removido - substituído por DashInDirection com animação melhorada

        /// <summary>
        /// Adiciona experiência a uma classe específica.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="experience">Quantidade de experiência a adicionar</param>
        public void AddClassExperience(string className, int experience)
        {
            SubClasses?.AddXPToSubClass(className, experience);

            // A notificação de XP agora pode ser acionada por um evento dentro do SubClassSystem se necessário,
            // ou podemos mantê-la aqui se quisermos um ponto de entrada único.
            RPGNotificationSystem.AddXPNotification(className, experience);
        }

        /// <summary>
        /// Adiciona experiência ao jogador.
        /// </summary>
        /// <param name="experience">Quantidade de experiência a adicionar</param>
        public void AddPlayerExperience(float experience)
        {
            PlayerExperience += experience;
            // TODO: Adicionar notificação de XP do jogador
            CheckPlayerLevelUp();
        }

        /// <summary>
        /// Verifica se o jogador subiu de nível geral.
        /// </summary>
        private void CheckPlayerLevelUp()
        {
            float expForNextLevel = GetPlayerExperienceForLevel(PlayerLevel + 1);
            if (PlayerExperience >= expForNextLevel)
            {
                PlayerLevel++;
                PlayerExperience -= expForNextLevel;
                AttributePoints += 5; // Ganha 5 pontos de atributo por nível
                
                // Desbloquear skills de movimentação
                UnlockMovementSkills();
                
                // Notificação global para todos
                Main.NewText($"You leveled up to level {PlayerLevel}! You gained 5 attribute points!", Color.Gold);
                SoundEngine.PlaySound(SoundID.Item37, Player.position);
                CheckPlayerLevelUp(); // Recursivamente verifica se subiu múltiplos níveis
            }
        }

        /// <summary>
        /// Desbloqueia skills de movimentação baseado no nível do jogador.
        /// </summary>
        private void UnlockMovementSkills()
        {
            // Double Jump no nível 3
            if (PlayerLevel == 3 && !UnlockedDoubleJump)
            {
                UnlockedDoubleJump = true;
                Main.NewText("🎯 Double Jump unlocked!", Color.Cyan);
                SoundEngine.PlaySound(SoundID.Item4, Player.position);
            }
            
            // Wall Jump no nível 4
            if (PlayerLevel == 4 && !UnlockedWallJump)
            {
                UnlockedWallJump = true;
                Main.NewText("🎯 Wall Jump unlocked!", Color.Cyan);
                SoundEngine.PlaySound(SoundID.Item4, Player.position);
            }
            
            // Dash aprimorado no nível 5
            if (PlayerLevel == 5)
            {
                DashCooldown = Math.Max(30, DashCooldown - 10); // Reduz cooldown
                Main.NewText("⚡ Dash improved! Cooldown reduced!", Color.Cyan);
                SoundEngine.PlaySound(SoundID.Item4, Player.position);
            }
        }

        /// <summary>
        /// Calcula a experiência necessária para um nível geral específico do jogador.
        /// </summary>
        /// <param name="level">Nível desejado</param>
        /// <returns>Experiência necessária</returns>
        public static float GetPlayerExperienceForLevel(int level)
        {
            // Fórmula: 1000 * level^2 (exemplo, pode ser ajustado)
            return 1000f * (float)Math.Pow(level, 2);
        }

        /// <summary>
        /// Verifica se o jogador subiu de nível em uma classe.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="oldLevel">Nível anterior</param>
        // Os métodos CheckClassLevelUp, GetExperienceForLevel, CheckAbilityUnlock e CheckMilestoneUnlock foram removidos
        // pois essa lógica agora é gerenciada internamente pelo PlayerSubClass e SubClassSystem.

        /// <summary>
        /// Consome uma porcentagem da stamina atual.
        /// </summary>
        /// <param name="percent">Porcentagem da stamina máxima a consumir (0-100)</param>
        /// <returns>True se conseguiu consumir a stamina, false se não há stamina suficiente</returns>
        public bool ConsumeStaminaPercent(float percent)
        {
            float cost = 100f * (percent / 100f); // 100f é a stamina máxima
            if (CurrentStamina >= cost)
            {
                CurrentStamina -= cost;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sistema de detecção de comida para restaurar fome.
        /// </summary>
        public void OnPlayerEatFood(Item foodItem)
        {
            // Restaurar fome baseado no tipo de comida
            float hungerRestore = GetFoodHungerValue(foodItem);
            CurrentHunger = Math.Min(100f, CurrentHunger + hungerRestore);
            
            // Feedback visual
            Main.NewText($"Ate {foodItem.Name}! Hunger: {CurrentHunger:F0}%", Color.Green);
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item2, Player.position);
        }
        
        /// <summary>
        /// Obtém o valor de restauração de fome de um item de comida.
        /// </summary>
        /// <param name="item">Item de comida</param>
        /// <returns>Valor de restauração de fome</returns>
        private float GetFoodHungerValue(Item item)
        {
            string itemName = item.Name.ToLower();
            
            // Comidas básicas (10-20%)
            if (itemName.Contains("apple") || itemName.Contains("berry") || 
                itemName.Contains("fruit") || itemName.Contains("vegetable"))
            {
                return 15f;
            }
            
            // Comidas intermediárias (20-35%)
            if (itemName.Contains("bread") || itemName.Contains("salad") || 
                itemName.Contains("soup") || itemName.Contains("stew"))
            {
                return 25f;
            }
            
            // Comidas principais (35-50%)
            if (itemName.Contains("meat") || itemName.Contains("fish") || 
                itemName.Contains("pizza") || itemName.Contains("sandwich") ||
                itemName.Contains("burger") || itemName.Contains("hot dog"))
            {
                return 40f;
            }
            
            // Comidas especiais (50-75%)
            if (itemName.Contains("cake") || itemName.Contains("pie") || 
                itemName.Contains("taco") || itemName.Contains("burrito"))
            {
                return 60f;
            }
            
            // Valor padrão para outras comidas
            return 20f;
        }

        /// <summary>
        /// Inicializa as skills de movimentação do jogador.
        /// </summary>
        public void InitializeMovementSkills()
        {
            MovementSkills.Clear();
            MovementSkills.Add(new Skills.Movement.MovementDashSkill());
            MovementSkills.Add(new Skills.Movement.DoubleJumpSkill());
            MovementSkills.Add(new Skills.Movement.WallJumpSkill());
        }

        /// <summary>
        /// Obtém uma skill específica por tipo.
        /// </summary>
        /// <typeparam name="T">Tipo da skill</typeparam>
        /// <returns>A skill encontrada ou null</returns>
        public T GetSkill<T>() where T : BaseSkill
        {
            return MovementSkills.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Atualiza todas as skills de movimentação.
        /// </summary>
        public void UpdateMovementSkills()
        {
            foreach (var skill in MovementSkills)
            {
                skill.Update(Player);
            }
        }

        /// <summary>
        /// Obtém a direção do input do jogador.
        /// </summary>
        /// <returns>Vetor normalizado da direção do input</returns>
        private Vector2 GetInputDirection()
        {
            Vector2 dir = Vector2.Zero;
            if (Player.controlUp) dir.Y -= 1;
            if (Player.controlDown) dir.Y += 1;
            if (Player.controlLeft) dir.X -= 1;
            if (Player.controlRight) dir.X += 1;

            if (dir != Vector2.Zero)
                dir.Normalize();

            return dir;
        }

        /// <summary>
        /// Executa dash na direção especificada (Sistema Souls-like).
        /// </summary>
        /// <param name="direction">Direção do dash (normalizada)</param>
        private void ExecuteDash(Vector2 direction)
        {
            // Verificar se já está fazendo dash
            if (isDashing) return;
            
            // Normalizar direção
            direction.Normalize();
            
            // Aplicar velocidade do dash
            Player.velocity = direction * DASH_SPEED;
            
            // Configurar timer e estado
            dashTimer = DASH_DURATION;
            dashDirection = direction;
            isDashing = true;
            
            // Resetar rotação inicial
            Player.fullRotation = 0f;
            
            // Aplicar invencibilidade
            Player.immune = true;
            Player.immuneTime = 10; // 10 frames de invencibilidade
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item24, Player.position);
            
            // Efeito visual inicial
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustDirect(
                    Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                    0, 0, DustID.Smoke,
                    -direction.X * 2f, -direction.Y * 2f, 
                    100, Color.White, 1.2f
                );
            }
            
            // Feedback visual do dash
            Main.NewText($"Dash! Stamina: {CurrentStamina:F0}", Color.Cyan);
        }

        /// <summary>
        /// Verifica se o jogador está tocando uma parede.
        /// </summary>
        /// <param name="side">Lado da parede (-1 esquerda, 1 direita)</param>
        /// <returns>True se está tocando uma parede</returns>
        public bool IsTouchingWall(out int side)
        {
            side = 0;
            var pos = Player.position;
            var h = Player.height;
            
            if (Collision.SolidCollision(pos + new Vector2(-2, 0), 2, h))
                side = -1;
            else if (Collision.SolidCollision(pos + new Vector2(Player.width, 0), 2, h))
                side = 1;
                
            return side != 0;
        }

        /// <summary>
        /// Processa os efeitos especiais das milestones ativas.
        /// </summary>
        private void ProcessMilestoneEffects()
        {
            // Milestone effects are now handled within the SubClassSystem's UpdateAllSkills or specific subclass logic.
        }

        /// <summary>
        /// Determina o tipo de armadura equipada pelo jogador.
        /// </summary>
        /// <returns>Tipo de armadura equipada</returns>
        public ArmorType GetEquippedArmorType()
        {
            // Determinar tipo baseado na armadura equipada
            Item helmet = Player.armor[0];
            Item chestplate = Player.armor[1];
            Item leggings = Player.armor[2];
            
            if (!helmet.IsAir || !chestplate.IsAir || !leggings.IsAir)
            {
                // Lógica para determinar tipo (simplificada)
                if (IsMagicArmor(helmet, chestplate, leggings))
                    return ArmorType.MagicRobes;
                else if (IsHeavyArmor(helmet, chestplate, leggings))
                    return ArmorType.Heavy;
                else
                    return ArmorType.Light;
            }
            
            return ArmorType.None;
        }

        /// <summary>
        /// Adiciona XP à proficiência de um tipo de armadura.
        /// </summary>
        /// <param name="armorType">Tipo de armadura</param>
        /// <param name="xp">Quantidade de XP a adicionar</param>
        public void GainArmorProficiencyXP(ArmorType armorType, float xp)
        {
            if (!ArmorProficiencyExperience.ContainsKey(armorType))
                ArmorProficiencyExperience[armorType] = 0f;
            if (!ArmorProficiencyLevels.ContainsKey(armorType))
                ArmorProficiencyLevels[armorType] = 1;
            
            ArmorProficiencyExperience[armorType] += xp;
            
            // Verificar level up
            float xpNeeded = GetArmorXPNeeded(ArmorProficiencyLevels[armorType]);
            if (ArmorProficiencyExperience[armorType] >= xpNeeded)
            {
                ArmorProficiencyLevels[armorType]++;
                ArmorProficiencyExperience[armorType] -= xpNeeded;
                
                // Feedback visual de level up
                ShowArmorLevelUpEffect(armorType);
            }
        }

        /// <summary>
        /// Adiciona XP à proficiência de um tipo de arma.
        /// </summary>
        /// <param name="weaponType">Tipo de arma</param>
        /// <param name="xp">Quantidade de XP a adicionar</param>
        public void AddWeaponProficiencyXP(WeaponType weaponType, float xp)
        {
            if (!WeaponProficiencyExperience.ContainsKey(weaponType))
                WeaponProficiencyExperience[weaponType] = 0f;
            if (!WeaponProficiencyLevels.ContainsKey(weaponType))
                WeaponProficiencyLevels[weaponType] = 1;
            
            WeaponProficiencyExperience[weaponType] += xp;
            
            // Verificar level up
            float xpNeeded = GetWeaponXPNeeded(WeaponProficiencyLevels[weaponType]);
            if (WeaponProficiencyExperience[weaponType] >= xpNeeded)
            {
                WeaponProficiencyLevels[weaponType]++;
                WeaponProficiencyExperience[weaponType] -= xpNeeded;
                
                // Feedback visual de level up
                ShowWeaponLevelUpEffect(weaponType);
            }
        }

        /// <summary>
        /// Calcula o XP necessário para o próximo nível de proficiência.
        /// </summary>
        /// <param name="level">Nível atual</param>
        /// <returns>XP necessário para o próximo nível</returns>
        private float GetArmorXPNeeded(int level)
        {
            return 100f + (level * 50f); // XP cresce com o nível
        }

        /// <summary>
        /// Calcula o XP necessário para o próximo nível de proficiência de arma.
        /// </summary>
        /// <param name="level">Nível atual</param>
        /// <returns>XP necessário para o próximo nível</returns>
        private float GetWeaponXPNeeded(int level)
        {
            return 100f + (level * 50f); // XP cresce com o nível
        }

        /// <summary>
        /// Verifica se a armadura equipada é do tipo mágico.
        /// </summary>
        /// <param name="helmet">Capacete</param>
        /// <param name="chest">Peitoral</param>
        /// <param name="legs">Calças</param>
        /// <returns>True se é armadura mágica</returns>
        private bool IsMagicArmor(Item helmet, Item chest, Item legs)
        {
            // Verificar se é armadura mágica (Mana bonus, etc.)
            bool hasManaBonus = helmet.manaIncrease > 0 || chest.manaIncrease > 0 || legs.manaIncrease > 0;
            bool hasMagicName = helmet.Name.ToLower().Contains("robe") || chest.Name.ToLower().Contains("robe") || legs.Name.ToLower().Contains("robe") ||
                       helmet.Name.ToLower().Contains("wizard") || chest.Name.ToLower().Contains("wizard") || legs.Name.ToLower().Contains("wizard") ||
                       helmet.Name.ToLower().Contains("mage") || chest.Name.ToLower().Contains("mage") || legs.Name.ToLower().Contains("mage") ||
                       helmet.Name.ToLower().Contains("sorcerer") || chest.Name.ToLower().Contains("sorcerer") || legs.Name.ToLower().Contains("sorcerer") ||
                       helmet.Name.ToLower().Contains("mystic") || chest.Name.ToLower().Contains("mystic") || legs.Name.ToLower().Contains("mystic");
    
            return hasManaBonus || hasMagicName;
        }

        /// <summary>
        /// Verifica se a armadura equipada é do tipo pesado.
        /// </summary>
        /// <param name="helmet">Capacete</param>
        /// <param name="chest">Peitoral</param>
        /// <param name="legs">Calças</param>
        /// <returns>True se é armadura pesada</returns>
        private bool IsHeavyArmor(Item helmet, Item chest, Item legs)
        {
            // Verificar se é armadura pesada (alta defesa)
            int totalDefense = helmet.defense + chest.defense + legs.defense;
            bool hasHighDefense = totalDefense >= 15; // Threshold para armadura pesada
            bool hasHeavyName = helmet.Name.ToLower().Contains("plate") || chest.Name.ToLower().Contains("plate") || legs.Name.ToLower().Contains("plate") ||
                       helmet.Name.ToLower().Contains("heavy") || chest.Name.ToLower().Contains("heavy") || legs.Name.ToLower().Contains("heavy") ||
                       helmet.Name.ToLower().Contains("titanium") || chest.Name.ToLower().Contains("titanium") || legs.Name.ToLower().Contains("titanium") ||
                       helmet.Name.ToLower().Contains("adamantite") || chest.Name.ToLower().Contains("adamantite") || legs.Name.ToLower().Contains("adamantite") ||
                       helmet.Name.ToLower().Contains("cobalt") || chest.Name.ToLower().Contains("cobalt") || legs.Name.ToLower().Contains("cobalt") ||
                       helmet.Name.ToLower().Contains("mythril") || chest.Name.ToLower().Contains("mythril") || legs.Name.ToLower().Contains("mythril") ||
                       helmet.Name.ToLower().Contains("orichalcum") || chest.Name.ToLower().Contains("orichalcum") || legs.Name.ToLower().Contains("orichalcum");
    
            return hasHighDefense || hasHeavyName;
        }

        /// <summary>
        /// Mostra efeito visual de level up de proficiência.
        /// </summary>
        /// <param name="armorType">Tipo de armadura que subiu de nível</param>
        private void ShowArmorLevelUpEffect(ArmorType armorType)
        {
            // Notificação global para todos
            Main.NewText($"Proficiency with {armorType} increased to level {ArmorProficiencyLevels[armorType]}!", Color.LightBlue);
        }

        /// <summary>
        /// Mostra efeito visual de level up de proficiência de arma.
        /// </summary>
        /// <param name="weaponType">Tipo de arma que subiu de nível</param>
        private void ShowWeaponLevelUpEffect(WeaponType weaponType)
        {
            // Notificação global para todos
            Main.NewText($"Proficiency with {weaponType} increased to level {WeaponProficiencyLevels[weaponType]}!", Color.LightGreen);
        }

        /// <summary>
        /// Obtém o nome de exibição da classe.
        /// </summary>
        private string GetClassNameDisplay(string className)
        {
            return className switch
            {
                "warrior" => "Warrior",
                "archer" => "Archer",
                "mage" => "Mage",
                "summoner" => "Summoner",
                "acrobat" => "Acrobat",
                "explorer" => "Explorer",
                "engineer" => "Engineer",
                "survivalist" => "Survivalist",
                "blacksmith" => "Blacksmith",
                "alchemist" => "Alchemist",
                "mystic" => "Mystic",
                _ => className
            };
        }

        public override void ResetEffects()
        {
            AutoDashEnabled = false;
            
            // Aplicar cálculos de atributos primários e outros bônus ⭐ NOVO
            var totalStats = RPGCalculations.CalculateTotalStats(this);
            RPGCalculations.ApplyStatsToPlayer(Player, totalStats);
        }
    }

    /// <summary>
    /// Enum para tipos de armadura.
    /// </summary>
    public enum ArmorType
    {
        None,
        Light,      // Armadura Leve - velocidade
        Heavy,      // Armadura Pesada - defesa
        MagicRobes  // Vestes Mágicas - mana
    }

    /// <summary>
    /// Enum para tipos de arma.
    /// </summary>
    public enum WeaponType
    {
        None,
        Melee,      // Corpo a corpo (espadas, lanças, etc.)
        Ranged,     // À distância (arcos, armas, etc.)
        Magic,      // Mágica (cajados, livros, etc.)
        Summon      // Invocação (chicotes, cajados de invocação)
    }
}
 