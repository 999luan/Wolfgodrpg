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
        public int DashCooldown { get; set; } = 0; // Resetado para usar novo sistema
        
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
        /// Número máximo de dashes disponíveis.
        /// </summary>
        public int MaxDashes { get; set; } = 1;

        // === SISTEMA DE CLASSES ===
        // OBSOLETE: A lógica de classes, níveis e experiência foi movida para SubClassSystem.
        // public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        // public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        // public List<ClassAbility> UnlockedAbilities = new List<ClassAbility>();

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
        private bool usedDoubleJump = false;

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

        // Flag para autodash (será ativada pelo item)
        public bool AutoDashEnabled = false;
        // Timers para double-tap em 4 direções
        private int leftTapTimer = 0, rightTapTimer = 0, upTapTimer = 0, downTapTimer = 0;
        private const int DoubleTapTime = 15;

        // Variáveis para dash direcional
        private int dashTimer = 0;
        private Vector2 dashDirection = Vector2.Zero;
        private float dashStartRotation = 0f;
        private float dashTargetRotation = 0f;
        private float dashRollProgress = 0f; // Progresso da rolagem (0-1)
        private bool isDashing = false; // Flag para indicar se está fazendo dash

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
            ProcessMilestoneEffects();
            UpdateMovementSkills();
            
            // Atualizar skills das subclasses
            SubClasses?.UpdateAllSkills();
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
        /// Processa triggers de input para o sistema de dash.
        /// </summary>
        /// <param name="triggersSet">Conjunto de triggers ativos</param>
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // === DASH DIRECIONAL NO MODO DE COMBATE ===
            if (CombatModeActive && UnlockedDash && DashCooldown <= 0)
            {
                Vector2 direction = GetInputDirection();
                if (direction != Vector2.Zero)
                {
                    if (ConsumeStaminaPercent(10f))
                    {
                        DashInDirection(direction);
                        DashCooldown = 60; // 1 segundo
                    }
                }
            }

            // === SISTEMA LEGADO DE SKILLS (DESATIVADO) ===
            // Comentado para evitar conflitos com o novo sistema
            /*
            if (!CombatModeActive) // Só usa o sistema legado se modo de combate estiver desativado
            {
                // Se autodash estiver ativo, dash ao segurar
                if (AutoDashEnabled)
                {
                    int dirX = 0, dirY = 0;
                    if (Player.controlLeft) dirX--;
                    if (Player.controlRight) dirX++;
                    if (Player.controlUp) dirY--;
                    if (Player.controlDown) dirY++;
                    if ((dirX != 0 || dirY != 0) && DashCooldown <= 0 && DashesUsed < MaxDashes && CurrentStamina >= 20f && dashTimer == 0)
                    {
                        PerformDash(new Vector2(dirX, dirY));
                    }
                    return;
                }

                // Double-tap para 4 direções - Lógica corrigida para evitar ativação acidental
                // Controle de skills de movimentação
                foreach (var skill in MovementSkills)
                {
                    if (skill is Skills.Movement.MovementDashSkill dashSkill)
                    {
                        // Dash com double-tap
                        if (Player.controlLeft)
                        {
                            if (leftTapTimer == 0)
                                leftTapTimer = DoubleTapTime;
                            else if (leftTapTimer > 0 && leftTapTimer < DoubleTapTime)
                            {
                                dashSkill.Activate(Player);
                                leftTapTimer = 0;
                            }
                            else
                                leftTapTimer--;
                        }
                        else
                        {
                            leftTapTimer = 0;
                        }

                        if (Player.controlRight)
                        {
                            if (rightTapTimer == 0)
                                rightTapTimer = DoubleTapTime;
                            else if (rightTapTimer > 0 && rightTapTimer < DoubleTapTime)
                            {
                                dashSkill.Activate(Player);
                                rightTapTimer = 0;
                            }
                            else
                                rightTapTimer--;
                        }
                        else
                        {
                            rightTapTimer = 0;
                        }
                    }
                    else if (skill is Skills.Movement.DoubleJumpSkill doubleJumpSkill)
                    {
                        // Double jump com tecla de pulo
                        if (Player.controlJump && !Player.velocity.Y.Equals(0))
                        {
                            doubleJumpSkill.Activate(Player);
                        }
                    }
                    else if (skill is Skills.Movement.WallJumpSkill wallJumpSkill)
                    {
                        // Wall jump com tecla de pulo
                        if (Player.controlJump)
                        {
                            wallJumpSkill.Activate(Player);
                        }
                    }
                }
            }
            */
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
            
            // Enviar dados de classe
            packet.Write(ClassLevels.Count);
            foreach (var kvp in ClassLevels)
            {
                packet.Write(kvp.Key);
                packet.Write(kvp.Value);
            }
            
            packet.Write(ClassExperience.Count);
            foreach (var kvp in ClassExperience)
            {
                packet.Write(kvp.Key);
                packet.Write(kvp.Value);
            }
            
            // Enviar habilidades desbloqueadas
            packet.Write(UnlockedAbilities.Count);
            foreach (var ability in UnlockedAbilities)
            {
                packet.Write((int)ability);
            }

            // Enviar proficiências de armadura
            packet.Write(ArmorProficiencyLevels.Count);
            foreach (var kvp in ArmorProficiencyLevels)
            {
                packet.Write((byte)kvp.Key);
                packet.Write(kvp.Value);
            }
            packet.Write(ArmorProficiencyExperience.Count);
            foreach (var kvp in ArmorProficiencyExperience)
            {
                packet.Write((byte)kvp.Key);
                packet.Write(kvp.Value);
            }

            // Enviar proficiências de arma
            packet.Write(WeaponProficiencyLevels.Count);
            foreach (var kvp in WeaponProficiencyLevels)
            {
                packet.Write((byte)kvp.Key);
                packet.Write(kvp.Value);
            }
            packet.Write(WeaponProficiencyExperience.Count);
            foreach (var kvp in WeaponProficiencyExperience)
            {
                packet.Write((byte)kvp.Key);
                packet.Write(kvp.Value);
            }
            
            packet.Send(toWho, fromWho);
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
            
            // Copiar dados de classe
            clientRPGPlayer.ClassLevels = new Dictionary<string, float>(ClassLevels);
            clientRPGPlayer.ClassExperience = new Dictionary<string, float>(ClassExperience);
            clientRPGPlayer.UnlockedAbilities = new List<ClassAbility>(UnlockedAbilities);
            clientRPGPlayer.ArmorProficiencyLevels = new Dictionary<ArmorType, int>(ArmorProficiencyLevels);
            clientRPGPlayer.ArmorProficiencyExperience = new Dictionary<ArmorType, float>(ArmorProficiencyExperience);
            clientRPGPlayer.WeaponProficiencyLevels = new Dictionary<WeaponType, int>(WeaponProficiencyLevels);
            clientRPGPlayer.WeaponProficiencyExperience = new Dictionary<WeaponType, float>(WeaponProficiencyExperience);
        }

        /// <summary>
        /// Verifica se houve mudanças significativas que precisam ser sincronizadas.
        /// </summary>
        /// <param name="clientPlayer">Jogador do cliente para comparação</param>
        /// <returns>True se houve mudanças significativas</returns>
        private bool HasSignificantChanges(RPGPlayer clientPlayer)
        {
            bool vitalsChanged = Math.Abs(CurrentHunger - clientPlayer.CurrentHunger) > 1f ||
                                 Math.Abs(CurrentSanity - clientPlayer.CurrentSanity) > 1f ||
                                 Math.Abs(CurrentStamina - clientPlayer.CurrentStamina) > 1f;

            bool classLevelsChanged = ClassLevels.Any(kvp => !clientPlayer.ClassLevels.ContainsKey(kvp.Key) || Math.Abs(kvp.Value - clientPlayer.ClassLevels[kvp.Key]) > 0.01f);
            bool classExpChanged = ClassExperience.Any(kvp => !clientPlayer.ClassExperience.ContainsKey(kvp.Key) || Math.Abs(kvp.Value - clientPlayer.ClassExperience[kvp.Key]) > 0.01f);
            bool unlockedAbilitiesChanged = UnlockedAbilities.Count != clientPlayer.UnlockedAbilities.Count || UnlockedAbilities.Except(clientPlayer.UnlockedAbilities).Any();

            bool armorLevelsChanged = ArmorProficiencyLevels.Any(kvp => !clientPlayer.ArmorProficiencyLevels.ContainsKey(kvp.Key) || kvp.Value != clientPlayer.ArmorProficiencyLevels[kvp.Key]);
            bool armorExpChanged = ArmorProficiencyExperience.Any(kvp => !clientPlayer.ArmorProficiencyExperience.ContainsKey(kvp.Key) || Math.Abs(kvp.Value - clientPlayer.ArmorProficiencyExperience[kvp.Key]) > 0.01f);

            bool weaponLevelsChanged = WeaponProficiencyLevels.Any(kvp => !clientPlayer.WeaponProficiencyLevels.ContainsKey(kvp.Key) || kvp.Value != clientPlayer.WeaponProficiencyLevels[kvp.Key]);
            bool weaponExpChanged = WeaponProficiencyExperience.Any(kvp => !clientPlayer.WeaponProficiencyExperience.ContainsKey(kvp.Key) || Math.Abs(kvp.Value - clientPlayer.WeaponProficiencyExperience[kvp.Key]) > 0.01f);

            return vitalsChanged || classLevelsChanged || classExpChanged || unlockedAbilitiesChanged ||
                   armorLevelsChanged || armorExpChanged || weaponLevelsChanged || weaponExpChanged;
        }

        /// <summary>
        /// Atualiza o sistema de vitais do jogador.
        /// </summary>
        private void UpdateVitals()
        {
            if (Vitals == null) return;
            
            // Regeneração de vitals usando o sistema modular
            Vitals.RegenerateHunger(0.016f); // 60 FPS
            Vitals.RegenerateSanity(0.016f);
            Vitals.RegenerateStamina(0.016f);
            
            // Aplicar efeitos baseados nos vitals
            ApplyVitalEffects();
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
        /// Atualiza o sistema de dash.
        /// </summary>
        private void UpdateDash()
        {
            if (DashCooldown > 0)
                DashCooldown--;
            if (DashResetTimer > 0)
            {
                DashResetTimer--;
                if (DashResetTimer <= 0)
                    DashesUsed = 0;
            }
            
            if (dashTimer > 0)
            {
                // Calcular progresso da animação (0-1)
                float progress = 1f - (dashTimer / (float)DashDuration);
                
                // Criar uma animação de rolagem suave usando função de easing
                // Usar uma curva de easing para tornar a animação mais natural
                float easedProgress = EaseInOutCubic(progress);
                
                // Calcular rotação baseada no progresso
                float rotationAngle = easedProgress * MathHelper.TwoPi; // 360 graus em radianos
                
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
                if (isDashing && Main.rand.NextBool(3)) // 33% de chance por frame
                {
                    // Criar partículas de velocidade
                    Dust.NewDustDirect(
                        Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                        0, 0, DustID.Smoke, 
                        -dashDirection.X * 2f, -dashDirection.Y * 2f, 
                        100, Color.White, 0.8f
                    );
                }
                
                dashTimer--;
                if (dashTimer == 0)
                {
                    // Finalizar animação
                    Player.fullRotation = 0f;
                    isDashing = false;
                    dashRollProgress = 0f;
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
        /// Executa dash na direção especificada.
        /// </summary>
        /// <param name="direction">Direção do dash</param>
        private void DashInDirection(Vector2 direction)
        {
            // Verificar se pode fazer dash
            if (DashCooldown > 0 || DashesUsed >= MaxDashes || CurrentStamina < 20f || direction == Vector2.Zero)
                return;
            
            direction.Normalize();
            
            // Consumir stamina
            CurrentStamina -= 20f;
            
            // Aplicar velocidade do dash
            Player.velocity = direction * DashSpeed;
            
            // Configurar cooldown e contadores
            DashCooldown = 60; // 1 segundo
            DashesUsed++;
            DashResetTimer = 180;
            
            // Inicializar animação de rolagem
            dashTimer = DashDuration;
            dashDirection = direction;
            dashRollProgress = 0f;
            isDashing = true;
            
            // Resetar rotação inicial
            Player.fullRotation = 0f;
            
            // Aplicar invencibilidade
            Player.immune = true;
            Player.immuneTime = DashInvincibilityFrames;
            
            // Efeito sonoro
            SoundEngine.PlaySound(SoundID.Item24, Player.position);
            
            // Efeito visual inicial
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustDirect(
                    Player.position + new Vector2(Main.rand.Next(Player.width), Main.rand.Next(Player.height)),
                    0, 0, DustID.Smoke,
                    -direction.X * 3f, -direction.Y * 3f, 
                    150, Color.White, 1.5f
                );
            }
        }

        /// <summary>
        /// Verifica se o jogador está tocando uma parede.
        /// </summary>
        /// <param name="side">Lado da parede (-1 esquerda, 1 direita)</param>
        /// <returns>True se está tocando uma parede</returns>
        private bool IsTouchingWall(out int side)
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
            foreach (var classEntry in ClassLevels)
            {
                string className = classEntry.Key;
                float classLevel = classEntry.Value;
                
                if (classLevel > 0)
                {
                    RPGMilestoneEffects.ProcessSpecialEffects(this, className, classLevel);
                }
            }
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
 