using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Wolfgodrpg.Common.Classes;
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
        public int DashCooldown { get; set; }
        
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
        /// <summary>
        /// Dicionário com os níveis de cada classe do jogador.
        /// </summary>
        public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        
        /// <summary>
        /// Dicionário com a experiência atual de cada classe.
        /// </summary>
        public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        
        /// <summary>
        /// Lista de habilidades desbloqueadas pelo jogador.
        /// </summary>
        public List<ClassAbility> UnlockedAbilities = new List<ClassAbility>();

        // === SISTEMA DE VITAIS ===
        /// <summary>
        /// Fome atual do jogador (0-100).
        /// </summary>
        public float CurrentHunger { get; set; } = 100f;
        
        /// <summary>
        /// Sanidade atual do jogador (0-100).
        /// </summary>
        public float CurrentSanity { get; set; } = 100f;
        
        /// <summary>
        /// Stamina atual do jogador (0-100).
        /// </summary>
        public float CurrentStamina { get; set; } = 100f;
        
        /// <summary>
        /// Taxa de regeneração de fome por segundo.
        /// </summary>
        public float HungerRegenRate { get; set; } = 0.5f;
        
        /// <summary>
        /// Taxa de regeneração de sanidade por segundo.
        /// </summary>
        public float SanityRegenRate { get; set; } = 0.3f;
        
        /// <summary>
        /// Taxa de regeneração de stamina por segundo.
        /// </summary>
        public float StaminaRegenRate { get; set; } = 2.0f;

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

        /// <summary>
        /// Inicializa o jogador com valores padrão.
        /// </summary>
        public override void Initialize()
        {
            // Inicializar classes com nível 0
            foreach (var className in RPGClassDefinitions.ActionClasses.Keys)
            {
                ClassLevels[className] = 0f;
                ClassExperience[className] = 0f;
            }
            
            // Inicializar proficiências de armadura ⭐ NOVO
            foreach (ArmorType armorType in System.Enum.GetValues<ArmorType>())
            {
                ArmorProficiencyLevels[armorType] = 1;
                ArmorProficiencyExperience[armorType] = 0f;
            }
            
            // Resetar vitals
            CurrentHunger = 100f;
            CurrentSanity = 100f;
            CurrentStamina = 100f;
            
            // Inicializar atributos primários
            Strength = 10;
            Dexterity = 10;
            Intelligence = 10;
            Constitution = 10;
            Wisdom = 10;

            // Inicializar nível do jogador
            PlayerLevel = 1;
            PlayerExperience = 0f;
            AttributePoints = 0;
            
            // Resetar dash
            DashCooldown = 0;
            DashesUsed = 0;
            DashResetTimer = 0;
        }

        /// <summary>
        /// Atualiza o jogador a cada frame.
        /// </summary>
        public override void PostUpdate()
        {
            UpdateVitals();
            UpdateDash();
        }

        /// <summary>
        /// Processa triggers de input para o sistema de dash.
        /// </summary>
        /// <param name="triggersSet">Conjunto de triggers ativos</param>
        public override void ProcessTriggers(TriggersSet triggersSet)
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
            // Double-tap para 4 direções
            // Esquerda
            if (Player.controlLeft)
            {
                if (leftTapTimer > 0 && leftTapTimer < DoubleTapTime && DashCooldown <= 0 && DashesUsed < MaxDashes && CurrentStamina >= 20f && dashTimer == 0)
                {
                    PerformDash(new Vector2(-1, 0));
                    leftTapTimer = 0;
                }
                else if (Player.releaseLeft)
                {
                    leftTapTimer = 1;
                }
                else if (leftTapTimer > 0)
                {
                    leftTapTimer++;
                    if (leftTapTimer > DoubleTapTime) leftTapTimer = 0;
                }
            }
            else if (leftTapTimer > 0)
            {
                leftTapTimer++;
                if (leftTapTimer > DoubleTapTime) leftTapTimer = 0;
            }
            // Direita
            if (Player.controlRight)
            {
                if (rightTapTimer > 0 && rightTapTimer < DoubleTapTime && DashCooldown <= 0 && DashesUsed < MaxDashes && CurrentStamina >= 20f && dashTimer == 0)
                {
                    PerformDash(new Vector2(1, 0));
                    rightTapTimer = 0;
                }
                else if (Player.releaseRight)
                {
                    rightTapTimer = 1;
                }
                else if (rightTapTimer > 0)
                {
                    rightTapTimer++;
                    if (rightTapTimer > DoubleTapTime) rightTapTimer = 0;
                }
            }
            else if (rightTapTimer > 0)
            {
                rightTapTimer++;
                if (rightTapTimer > DoubleTapTime) rightTapTimer = 0;
            }
            // Cima
            if (Player.controlUp)
            {
                if (upTapTimer > 0 && upTapTimer < DoubleTapTime && DashCooldown <= 0 && DashesUsed < MaxDashes && CurrentStamina >= 20f && dashTimer == 0)
                {
                    PerformDash(new Vector2(0, -1));
                    upTapTimer = 0;
                }
                else if (Player.releaseUp)
                {
                    upTapTimer = 1;
                }
                else if (upTapTimer > 0)
                {
                    upTapTimer++;
                    if (upTapTimer > DoubleTapTime) upTapTimer = 0;
                }
            }
            else if (upTapTimer > 0)
            {
                upTapTimer++;
                if (upTapTimer > DoubleTapTime) upTapTimer = 0;
            }
            // Baixo
            if (Player.controlDown)
            {
                if (downTapTimer > 0 && downTapTimer < DoubleTapTime && DashCooldown <= 0 && DashesUsed < MaxDashes && CurrentStamina >= 20f && dashTimer == 0)
                {
                    PerformDash(new Vector2(0, 1));
                    downTapTimer = 0;
                }
                else if (Player.releaseDown)
                {
                    downTapTimer = 1;
                }
                else if (downTapTimer > 0)
                {
                    downTapTimer++;
                    if (downTapTimer > DoubleTapTime) downTapTimer = 0;
                }
            }
            else if (downTapTimer > 0)
            {
                downTapTimer++;
                if (downTapTimer > DoubleTapTime) downTapTimer = 0;
            }
        }

        /// <summary>
        /// Método público para ganhar XP de proficiência de armadura quando o jogador recebe dano.
        /// </summary>
        /// <param name="damage">Quantidade de dano recebido</param>
        public void OnPlayerDamaged(int damage)
        {
            if (damage > 0)
            {
                ArmorType currentArmorType = GetEquippedArmorType();
                if (currentArmorType != ArmorType.None)
                {
                    GainArmorProficiencyXP(currentArmorType, damage * 0.1f);
                }
            }
        }

        /// <summary>
        /// Salva os dados do jogador usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound para salvar os dados</param>
        public override void SaveData(TagCompound tag)
        {
            // Serializar ClassLevels
            var classLevelsList = ClassLevels.Select(kv => new TagCompound {
                ["key"] = kv.Key,
                ["value"] = kv.Value
            }).ToList();
            tag["ClassLevels"] = classLevelsList;

            // Serializar ClassExperience
            var classExpList = ClassExperience.Select(kv => new TagCompound {
                ["key"] = kv.Key,
                ["value"] = kv.Value
            }).ToList();
            tag["ClassExperience"] = classExpList;

            // Salvar habilidades desbloqueadas
            tag["UnlockedAbilities"] = UnlockedAbilities.Select(a => (int)a).ToList();
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
        }

        /// <summary>
        /// Carrega os dados do jogador usando TagCompound.
        /// </summary>
        /// <param name="tag">TagCompound contendo os dados salvos</param>
        public override void LoadData(TagCompound tag)
        {
            // Desserializar ClassLevels
            ClassLevels.Clear();
            if (tag.ContainsKey("ClassLevels"))
            {
                foreach (var entry in tag.GetList<TagCompound>("ClassLevels"))
                {
                    string key = entry.GetString("key");
                    float value = entry.GetFloat("value");
                    ClassLevels[key] = value;
                }
            }
            // Desserializar ClassExperience
            ClassExperience.Clear();
            if (tag.ContainsKey("ClassExperience"))
            {
                foreach (var entry in tag.GetList<TagCompound>("ClassExperience"))
                {
                    string key = entry.GetString("key");
                    float value = entry.GetFloat("value");
                    ClassExperience[key] = value;
                }
            }
            // Carregar habilidades desbloqueadas
            if (tag.ContainsKey("UnlockedAbilities"))
            {
                var abilityInts = tag.GetList<int>("UnlockedAbilities");
                UnlockedAbilities = abilityInts.Select(i => (ClassAbility)i).ToList();
            }
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
        }

        /// <summary>
        /// Verifica se houve mudanças significativas que precisam ser sincronizadas.
        /// </summary>
        /// <param name="clientPlayer">Jogador do cliente para comparação</param>
        /// <returns>True se houve mudanças significativas</returns>
        private bool HasSignificantChanges(RPGPlayer clientPlayer)
        {
            return Math.Abs(CurrentHunger - clientPlayer.CurrentHunger) > 1f ||
                   Math.Abs(CurrentSanity - clientPlayer.CurrentSanity) > 1f ||
                   Math.Abs(CurrentStamina - clientPlayer.CurrentStamina) > 1f;
        }

        /// <summary>
        /// Atualiza o sistema de vitais do jogador.
        /// </summary>
        private void UpdateVitals()
        {
            // Regeneração de vitals
            CurrentHunger = Math.Min(100f, CurrentHunger + HungerRegenRate * 0.016f); // 60 FPS
            CurrentSanity = Math.Min(100f, CurrentSanity + SanityRegenRate * 0.016f);
            CurrentStamina = Math.Min(100f, CurrentStamina + StaminaRegenRate * 0.016f);
            
            // Aplicar efeitos baseados nos vitals
            ApplyVitalEffects();
        }

        /// <summary>
        /// Aplica efeitos baseados no estado dos vitals.
        /// </summary>
        private void ApplyVitalEffects()
        {
            // Efeitos de fome baixa
            if (CurrentHunger < 20f)
            {
                Player.moveSpeed *= 0.8f;
                Player.jumpSpeedBoost *= 0.8f;
            }
            
            // Efeitos de sanidade baixa
            if (CurrentSanity < 30f)
            {
                Player.statDefense -= 5;
            }
            
            // Efeitos de stamina baixa
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
                // Girar o sprite durante o dash
                float t = 1f - (dashTimer / (float)DashDuration);
                Player.fullRotation = MathHelper.Lerp(dashStartRotation, dashTargetRotation, t);
                dashTimer--;
                if (dashTimer == 0)
                {
                    Player.fullRotation = 0f;
                }
            }
        }

        /// <summary>
        /// Executa o dash na direção especificada.
        /// </summary>
        /// <param name="direction">Direção do dash (-1 para esquerda, 1 para direita)</param>
        private void PerformDash(Vector2 direction)
        {
            if (DashCooldown > 0 || DashesUsed >= MaxDashes || CurrentStamina < 20f || direction == Vector2.Zero)
                return;
            direction.Normalize();
            CurrentStamina -= 20f;
            Player.velocity = direction * DashSpeed;
            DashCooldown = 30;
            DashesUsed++;
            DashResetTimer = 180;
            dashTimer = DashDuration;
            dashDirection = direction;
            dashStartRotation = Player.fullRotation;
            dashTargetRotation = Player.fullRotation + MathHelper.ToRadians(360f) * (direction.X < 0 ? -1 : 1);
            Player.immune = true;
            Player.immuneTime = DashInvincibilityFrames;
            SoundEngine.PlaySound(SoundID.Item24, Player.position);
        }

        /// <summary>
        /// Adiciona experiência a uma classe específica.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="experience">Quantidade de experiência a adicionar</param>
        public void AddClassExperience(string className, float experience)
        {
            if (!ClassExperience.ContainsKey(className))
                ClassExperience[className] = 0f;
            
            float oldLevel = ClassLevels.ContainsKey(className) ? ClassLevels[className] : 0f;
            ClassExperience[className] += experience;
            
            // Adicionar notificação de XP
            RPGNotificationSystem.AddXPNotification(className, experience);
            
            // Verificar se subiu de nível
            CheckClassLevelUp(className, oldLevel);
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
                // TODO: Adicionar notificação de level up do jogador
                Main.NewText($"Você subiu para o nível {PlayerLevel}! Você ganhou 5 pontos de atributo!", Color.Gold);
                SoundEngine.PlaySound(SoundID.Item37, Player.position);
                CheckPlayerLevelUp(); // Recursivamente verifica se subiu múltiplos níveis
            }
        }

        /// <summary>
        /// Calcula a experiência necessária para um nível geral específico do jogador.
        /// </summary>
        /// <param name="level">Nível desejado</param>
        /// <returns>Experiência necessária</returns>
        private float GetPlayerExperienceForLevel(int level)
        {
            // Fórmula: 1000 * level^2 (exemplo, pode ser ajustado)
            return 1000f * (float)Math.Pow(level, 2);
        }

        /// <summary>
        /// Verifica se o jogador subiu de nível em uma classe.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        /// <param name="oldLevel">Nível anterior</param>
        private void CheckClassLevelUp(string className, float oldLevel)
        {
            if (!ClassLevels.ContainsKey(className))
                ClassLevels[className] = 0f;
            
            float currentLevel = ClassLevels[className];
            float currentExp = ClassExperience[className];
            float expForNextLevel = GetExperienceForLevel(currentLevel + 1);
            
            if (currentExp >= expForNextLevel)
            {
                // Subir de nível
                ClassLevels[className]++;
                ClassExperience[className] -= expForNextLevel;
                
                // Adicionar notificação de level up
                RPGNotificationSystem.AddLevelUpNotification(className, (int)ClassLevels[className]);
                
                // Verificar se desbloqueou alguma habilidade
                CheckAbilityUnlock(className);
                
                // Efeitos de level up
                SoundEngine.PlaySound(SoundID.Item4, Player.position);
                
                // Mostrar texto de level up na tela
                Main.NewText($"{GetClassNameDisplay(className)} subiu para o nível {ClassLevels[className]}!", Color.Gold);
            }
        }

        /// <summary>
        /// Calcula a experiência necessária para um nível específico.
        /// </summary>
        /// <param name="level">Nível desejado</param>
        /// <returns>Experiência necessária</returns>
        private float GetExperienceForLevel(float level)
        {
            // Fórmula: 100 * level^1.5
            return 100f * (float)Math.Pow(level, 1.5);
        }

        /// <summary>
        /// Verifica se o jogador desbloqueou alguma habilidade ao subir de nível.
        /// </summary>
        /// <param name="className">Nome da classe</param>
        private void CheckAbilityUnlock(string className)
        {
            if (!RPGClassDefinitions.ActionClasses.ContainsKey(className))
                return;
            
            var classInfo = RPGClassDefinitions.ActionClasses[className];
            float currentLevel = ClassLevels[className];
            
            foreach (var milestone in classInfo.Milestones)
            {
                if (currentLevel >= (int)milestone.Key && !UnlockedAbilities.Contains(milestone.Key))
                {
                    UnlockedAbilities.Add(milestone.Key);
                    // TODO: Notificar o jogador sobre a nova habilidade
                }
            }
        }

        /// <summary>
        /// Determina o tipo de armadura equipada pelo jogador.
        /// </summary>
        /// <returns>Tipo de armadura equipada</returns>
        private ArmorType GetEquippedArmorType()
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
        private void GainArmorProficiencyXP(ArmorType armorType, float xp)
        {
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
        /// Calcula o XP necessário para o próximo nível de proficiência.
        /// </summary>
        /// <param name="level">Nível atual</param>
        /// <returns>XP necessário para o próximo nível</returns>
        private float GetArmorXPNeeded(int level)
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
            return helmet.manaIncrease > 0 || chest.manaIncrease > 0 || legs.manaIncrease > 0;
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
            return totalDefense >= 20; // Threshold para armadura pesada
        }

        /// <summary>
        /// Mostra efeito visual de level up de proficiência.
        /// </summary>
        /// <param name="armorType">Tipo de armadura que subiu de nível</param>
        private void ShowArmorLevelUpEffect(ArmorType armorType)
        {
            // Efeito visual e som de level up
            Main.NewText($"Proficiência com {armorType} aumentou para nível {ArmorProficiencyLevels[armorType]}!", 
                         Color.Gold);
        }

        /// <summary>
        /// Obtém o nome de exibição da classe.
        /// </summary>
        private string GetClassNameDisplay(string className)
        {
            return className switch
            {
                "warrior" => "Guerreiro",
                "archer" => "Arqueiro",
                "mage" => "Mago",
                "summoner" => "Invocador",
                "acrobat" => "Acrobata",
                "explorer" => "Explorador",
                "engineer" => "Engenheiro",
                "survivalist" => "Sobrevivente",
                "blacksmith" => "Ferreiro",
                "alchemist" => "Alquimista",
                "mystic" => "Místico",
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
}
 