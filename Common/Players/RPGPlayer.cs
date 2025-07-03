using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq; // Adicionado para o ToList()
using Wolfgodrpg.Common.Classes;
using Wolfgodrpg.Common.Systems;
using Terraria.DataStructures; // Para PlayerDeathReason

namespace Wolfgodrpg.Common.Players
{
    public class RPGPlayer : ModPlayer
    {
        // Constantes de balanceamento
        private const float BASE_XP_NEEDED = 100f;
        private const float XP_MULTIPLIER = 1.1f;

        // Sistema de Classes
        public Dictionary<string, float> ClassLevels = new Dictionary<string, float>();
        public Dictionary<string, float> ClassExperience = new Dictionary<string, float>();
        public HashSet<string> UnlockedAbilities = new HashSet<string>();

        // Vitals do Jogador (Fome, Sanidade, Stamina)
        public float CurrentHunger = 100f;
        public float MaxHunger = 100f;
        public float CurrentSanity = 100f;
        public float MaxSanity = 100f;
        public float CurrentStamina = 100f;
        public float MaxStamina = 100f;
        public int StaminaRegenDelay = 0;
        public int CombatTimer = 0; // Cronômetro para rastrear tempo em combate

        private bool _wasWellFed;

        // Configuração
        private RPGConfig Config => ModContent.GetInstance<RPGConfig>();

        // === SISTEMA DE DASH CUSTOMIZADO ===
        public int DashType = 0; // Tipo de dash (0 = nenhum, 1 = nosso dash customizado)
        public int DashCooldown = 0; // Cooldown entre dashes
        public int DashDelay = 0; // Delay antes do próximo dash
        public int DashDir = -1; // Direção do dash (-1 = esquerda, 1 = direita, 0 = parado)
        public int DashTimer = 0; // Timer para controlar o dash
        public int DashesUsed = 0; // Quantos dashes foram usados
        public int DashResetTimer = 0; // Timer para resetar os dashes
        
        // NOVA VARIÁVEL: Simula ter o item de dash
        public bool HasDashItem => GetMaxDashes() > 0; // Sempre true se tem nível de movimento >= 1
        
        // Constantes do dash
        private const int DASH_DURATION = 35; // Duração do dash em frames
        private const int DASH_COOLDOWN = 50; // Cooldown entre dashes
        private const int DASH_RESET_TIME = 180; // Tempo para resetar dashes (3 segundos)
        private const float DASH_VELOCITY = 16f; // Velocidade do dash
        
        // Calcula quantos dashes o jogador tem disponível baseado no nível
        public int GetMaxDashes()
        {
            float movementLevel = GetClassLevel("movement");
            if (movementLevel >= 100f) return 999; // Dash infinito
            if (movementLevel >= 90f) return 10;
            if (movementLevel >= 80f) return 9;
            if (movementLevel >= 70f) return 8;
            if (movementLevel >= 60f) return 7;
            if (movementLevel >= 50f) return 6;
            if (movementLevel >= 40f) return 5;
            if (movementLevel >= 30f) return 4;
            if (movementLevel >= 20f) return 3;
            if (movementLevel >= 10f) return 2;
            if (movementLevel >= 1f) return 1;
            return 0;
        }
        
        // Verifica se pode fazer dash
        public bool CanDash(int dir)
        {
            int maxDashes = GetMaxDashes();
            if (maxDashes == 0) {
                DebugLog.Player("CanDash", $"Dash negado - maxDashes é 0 (nível movimento: {GetClassLevel("movement"):F0})");
                return false;
            }
            if (maxDashes != 999 && DashesUsed >= maxDashes) {
                DebugLog.Player("CanDash", $"Dash negado - dashes esgotados. Usados: {DashesUsed}, Máximo: {maxDashes}");
                return false;
            }
            if (DashCooldown > 0) {
                DebugLog.Player("CanDash", $"Dash negado - em cooldown. Cooldown restante: {DashCooldown}");
                return false;
            }
            if (Player.mount.Active) {
                DebugLog.Player("CanDash", $"Dash negado - jogador montado");
                return false;
            }
            
            // GARANTIR QUE NOSSO DASH FUNCIONE INDEPENDENTEMENTE DE ACESSÓRIOS
            // O dash do nosso mod deve funcionar sempre que o jogador tiver nível de movimento >= 1
            // Não deve depender de Shield of Cthulhu ou outros acessórios
            
            DebugLog.Player("CanDash", $"✅ Dash permitido - maxDashes: {maxDashes}, usados: {DashesUsed}, cooldown: {DashCooldown}, HasDashItem: {HasDashItem}");
            return true;
        }
        
        // Executa o dash
        public void DoDash(int dir, int vdir = 0)
        {
            float staminaCost = 25f;
            if (!ConsumeStamina(staminaCost)) {
                DebugLog.Player("DoDash", $"Dash cancelado - stamina insuficiente. Necessário: {staminaCost}, Atual: {CurrentStamina}");
                return;
            }

            DashesUsed++;
            DashResetTimer = DASH_RESET_TIME;
            DashCooldown = DASH_COOLDOWN;

            // Dash horizontal
            if (dir != 0)
            {
                Player.velocity.X = DASH_VELOCITY * dir;
                Player.velocity.Y *= 0.5f; // Suaviza o dash horizontal
                DebugLog.Player("DoDash", $"Dash horizontal executado - Direção: {dir}, Velocidade: {DASH_VELOCITY * dir}");
            }
            // Dash vertical
            else if (vdir != 0)
            {
                Player.velocity.Y = DASH_VELOCITY * vdir;
                Player.velocity.X *= 0.5f; // Suaviza o dash vertical
                DebugLog.Player("DoDash", $"Dash vertical executado - Direção: {vdir}, Velocidade: {DASH_VELOCITY * vdir}");
            }

            // Efeito visual vanilla opcional (apenas para feedback visual)
            Player.dashType = Terraria.ID.DashID.ShieldOfCthulhu;
            
            DebugLog.Player("DoDash", $"Dash completo - Dashes usados: {DashesUsed}, Cooldown: {DashCooldown}, Reset timer: {DashResetTimer}");
        }

        public override void Initialize()
        {
            DebugLog.Player("Initialize", "Inicializando RPGPlayer");
            InitializeClasses();
            DebugLog.Player("Initialize", $"Dicionário de classes após inicialização: {string.Join(", ", ClassLevels.Select(kv => kv.Key + ":" + kv.Value))}");
            DebugLog.Player("Initialize", "RPGPlayer inicializado com sucesso");
        }

        private void InitializeClasses()
        {
            DebugLog.Player("InitializeClasses", "Iniciando inicialização das classes do jogador");
            DebugLog.Player("InitializeClasses", $"RPGClassDefinitions.ActionClasses.Count: {RPGClassDefinitions.ActionClasses.Count}");
            
            int classesInitialized = 0;
            foreach (var className in RPGClassDefinitions.ActionClasses.Keys)
            {
                if (!ClassLevels.ContainsKey(className))
                {
                    ClassLevels[className] = 1f;
                    DebugLog.Player("InitializeClasses", $"Classe '{className}' inicializada com level 1");
                    classesInitialized++;
                }
                else
                {
                    DebugLog.Player("InitializeClasses", $"Classe '{className}' já existe com level {ClassLevels[className]}");
                }

                if (!ClassExperience.ContainsKey(className))
                {
                    ClassExperience[className] = 0f;
                    DebugLog.Player("InitializeClasses", $"XP da classe '{className}' inicializado com 0");
                }
                else
                {
                    DebugLog.Player("InitializeClasses", $"XP da classe '{className}' já existe: {ClassExperience[className]}");
                }
            }
            
            DebugLog.Player("InitializeClasses", $"Total de classes inicializadas: {classesInitialized}");
            DebugLog.Player("InitializeClasses", $"ClassLevels.Count final: {ClassLevels.Count}");
            DebugLog.Player("InitializeClasses", $"ClassExperience.Count final: {ClassExperience.Count}");
            DebugLog.Player("InitializeClasses", $"Conteúdo do ClassLevels: {string.Join(", ", ClassLevels.Select(kv => kv.Key + ":" + kv.Value))}");
            
            // VERIFICAÇÃO ESPECÍFICA DO NÍVEL DE MOVIMENTO
            if (ClassLevels.ContainsKey("movement"))
            {
                float movementLevel = ClassLevels["movement"];
                int maxDashes = GetMaxDashes();
                DebugLog.Player("InitializeClasses", $"NÍVEL DE MOVIMENTO VERIFICADO - Level: {movementLevel}, Max dashes: {maxDashes}");
            }
            else
            {
                DebugLog.Error("Player", "InitializeClasses", "ERRO: Classe 'movement' não encontrada no ClassLevels!");
            }
        }

        public override void PostUpdate()
        {
            // Atualizar timer de combate
            if (CombatTimer > 0)
            {
                CombatTimer--;
                if (CombatTimer == 0)
                {
                    DebugLog.Player("PostUpdate", "Timer de combate expirado - jogador saiu do combate");
                }
            }

            // === SISTEMA DE DASH ATIVADO ===
            // GARANTIR QUE NOSSO DASH FUNCIONE INDEPENDENTEMENTE DE ACESSÓRIOS
            // O dash do nosso mod deve funcionar sempre que o jogador tiver nível de movimento >= 1
            // Não deve depender de Shield of Cthulhu ou outros acessórios
            
            int maxDashes = GetMaxDashes();
            if (maxDashes > 0)
            {
                // FORÇAR O DASH VANILLA PARA FUNCIONAR
                // Simular ter o Shield of Cthulhu sempre que tem nível de movimento >= 1
                Player.dashType = 1; // Ativar dash vanilla para permitir duplo-toque
                
                // Log para debug do dash customizado (mais frequente para debug)
                if (Main.GameUpdateCount % 30 == 0) // A cada meio segundo
                {
                    DebugLog.Player("PostUpdate", $"⚡ Dash customizado ATIVO - Nível movimento: {GetClassLevel("movement"):F0}, Max dashes: {maxDashes}, Usados: {DashesUsed}, Cooldown: {DashCooldown}, HasDashItem: {HasDashItem}");
                }
            }
            else
            {
                // Se não tem dash disponível, garantir que o dash vanilla também não esteja ativo
                if (Player.dashType == 1)
                {
                    Player.dashType = 0;
                    DebugLog.Player("PostUpdate", "Dash vanilla desativado - jogador não tem nível de movimento suficiente");
                }
                
                // Log de erro se não tem dash disponível
                if (Main.GameUpdateCount % 60 == 0) // A cada segundo
                {
                    DebugLog.Error("Player", "PostUpdate", $"❌ Dash não disponível - Nível movimento: {GetClassLevel("movement"):F0}, Max dashes: {maxDashes}");
                }
            }

            // Atualizar cooldowns do nosso sistema
            if (DashCooldown > 0) DashCooldown--;
            if (DashDelay > 0) DashDelay--;
            
            // Resetar dashes após um tempo
            if (DashResetTimer > 0)
            {
                DashResetTimer--;
                if (DashResetTimer == 0)
                {
                    DashesUsed = 0;
                    DebugLog.Player("PostUpdate", "Dashes resetados");
                }
            }

            // Verificar mudanças de status
            CheckStatusChanges();

            // Aplicar stats calculados ao jogador (importante para a aba de stats funcionar)
            var calculatedStats = RPGCalculations.CalculateTotalStats(this);
            RPGCalculations.ApplyStatsToPlayer(Player, calculatedStats);
        }

        private void CheckStatusChanges()
        {
            // Verificar mudanças de fome
            if (CurrentHunger < 20f && _wasWellFed)
            {
                DebugLog.Player("CheckStatusChanges", $"Fome crítica detectada: {CurrentHunger:F1}% - regeneração de vida desabilitada");
                _wasWellFed = false;
            }
            else if (CurrentHunger >= 20f && !_wasWellFed)
            {
                DebugLog.Player("CheckStatusChanges", $"Fome normalizada: {CurrentHunger:F1}% - regeneração de vida reabilitada");
                _wasWellFed = true;
            }

            // Verificar mudanças de sanidade
            if (CurrentSanity < 30f)
            {
                DebugLog.Player("CheckStatusChanges", $"Sanidade baixa: {CurrentSanity:F1}% - efeitos negativos podem ocorrer");
            }
        }

        public float GetClassLevel(string className)
        {
            if (ClassLevels.TryGetValue(className, out float level))
            {
                DebugLog.Player("GetClassLevel", $"Classe '{className}' encontrada, level: {level}");
                return level;
            }
            
            DebugLog.Warn("Player", "GetClassLevel", $"Classe '{className}' não encontrada no dicionário ClassLevels");
            DebugLog.Warn("Player", "GetClassLevel", $"Classes disponíveis: {string.Join(", ", ClassLevels.Keys)}");
            DebugLog.Warn("Player", "GetClassLevel", $"Retornando level 1 como fallback");
            return 1f;
        }

        public void GainClassExp(string className, float amount)
        {
            if (!ClassExperience.ContainsKey(className)) 
            {
                DebugLog.Error("Player", "GainClassExp", $"Classe '{className}' não encontrada no dicionário de experiência");
                return;
            }

            float oldXP = ClassExperience[className];
            float oldLevel = ClassLevels[className];

            float finalAmount = amount * Config.ExpMultiplier;
            if (Main.hardMode) finalAmount *= 1.5f;
            if (NPC.downedMoonlord) finalAmount *= 2f;

            ClassExperience[className] += finalAmount;
            float newXP = ClassExperience[className];

            DebugLog.Gameplay("Player", "GainClassExp", $"Classe: {className}, XP ganho: {finalAmount:F1}, XP total: {newXP:F1}, Level atual: {oldLevel:F1}");

            float newLevel = CalculateLevelFromXP(ClassExperience[className]);

            if (newLevel > oldLevel)
            {
                ClassLevels[className] = newLevel;
                DebugLog.Gameplay("Player", "GainClassExp", $"LEVEL UP! Classe: {className}, Level: {oldLevel:F1} -> {newLevel:F1}");
                Main.NewText($"Nível de {RPGClassDefinitions.ActionClasses[className].Name} aumentou para {newLevel:F0}!", Color.Yellow);
                CheckForNewAbilities(className, newLevel);
            }
        }

        private float CalculateLevelFromXP(float xp)
        {
            float level = 1;
            float xpForNextLevel = BASE_XP_NEEDED;
            while (xp >= xpForNextLevel)
            {
                xp -= xpForNextLevel;
                level++;
                xpForNextLevel *= XP_MULTIPLIER;
            }
            return level;
        }

        private void CheckForNewAbilities(string className, float level)
        {
            DebugLog.Player("CheckForNewAbilities", $"Verificando habilidades para classe '{className}' no nível {level:F1}");
            var milestones = RPGClassDefinitions.ActionClasses[className].Milestones;
            foreach (var milestone in milestones)
            {
                if (level >= milestone.Key)
                {
                    // Usar o texto da habilidade em vez de criar um ID
                    string abilityText = milestone.Value;
                    UnlockAbility(abilityText);
                }
            }
        }

        public void UnlockAbility(string ability)
        {
            if (UnlockedAbilities.Add(ability))
            {
                DebugLog.Gameplay("Player", "UnlockAbility", $"Nova habilidade desbloqueada: {ability}");
                Main.NewText($"Nova habilidade desbloqueada: {ability}!", Color.LightBlue);
            }
            else
            {
                DebugLog.Player("UnlockAbility", $"Habilidade '{ability}' já estava desbloqueada");
            }
        }

        public bool HasUnlockedAbility(string ability)
        {
            return UnlockedAbilities.Contains(ability);
        }

        // === SISTEMA DE STAMINA ===
        public bool ConsumeStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                float oldStamina = CurrentStamina;
                CurrentStamina -= amount;
                StaminaRegenDelay = 60; // 1 segundo de delay
                
                DebugLog.Player("ConsumeStamina", $"Stamina consumida: {amount:F1}, Stamina restante: {CurrentStamina:F1}/{MaxStamina:F1}");
                return true;
            }
            
            DebugLog.Player("ConsumeStamina", $"Stamina insuficiente: necessário {amount:F1}, disponível {CurrentStamina:F1}");
            return false;
        }

        public void RestoreStamina(float amount)
        {
            float oldStamina = CurrentStamina;
            CurrentStamina = System.Math.Min(MaxStamina, CurrentStamina + amount);
            
            if (CurrentStamina > oldStamina)
            {
                DebugLog.Player("RestoreStamina", $"Stamina restaurada: {amount:F1}, Stamina atual: {CurrentStamina:F1}/{MaxStamina:F1}");
            }
        }

        // === SAVE/LOAD ===
        public override void SaveData(TagCompound tag)
        {
            DebugLog.Player("SaveData", "Salvando dados do RPGPlayer");
            // Serializar dicionários como duas listas separadas
            tag["classLevelKeys"] = ClassLevels.Keys.ToList();
            tag["classLevelValues"] = ClassLevels.Values.ToList();
            tag["classExpKeys"] = ClassExperience.Keys.ToList();
            tag["classExpValues"] = ClassExperience.Values.ToList();
            tag["unlockedAbilities"] = UnlockedAbilities.ToList();
            tag["currentHunger"] = CurrentHunger;
            tag["maxHunger"] = MaxHunger;
            tag["currentSanity"] = CurrentSanity;
            tag["maxSanity"] = MaxSanity;
            tag["currentStamina"] = CurrentStamina;
            tag["maxStamina"] = MaxStamina;
            tag["dashesUsed"] = DashesUsed;
            tag["dashResetTimer"] = DashResetTimer;
            DebugLog.Player("SaveData", "Dados do RPGPlayer salvos com sucesso");
        }

        public override void LoadData(TagCompound tag)
        {
            DebugLog.Player("LoadData", "Carregando dados do RPGPlayer");
            try
            {
                // Reconstruir dicionários a partir de duas listas
                var keys = tag.Get<List<string>>("classLevelKeys") ?? new List<string>();
                var values = tag.Get<List<float>>("classLevelValues") ?? new List<float>();
                ClassLevels = new Dictionary<string, float>();
                for (int i = 0; i < keys.Count && i < values.Count; i++)
                    ClassLevels[keys[i]] = values[i];

                var expKeys = tag.Get<List<string>>("classExpKeys") ?? new List<string>();
                var expValues = tag.Get<List<float>>("classExpValues") ?? new List<float>();
                ClassExperience = new Dictionary<string, float>();
                for (int i = 0; i < expKeys.Count && i < expValues.Count; i++)
                    ClassExperience[expKeys[i]] = expValues[i];

                var unlockedAbilitiesList = tag.Get<List<string>>("unlockedAbilities") ?? new List<string>();
                UnlockedAbilities = new HashSet<string>(unlockedAbilitiesList);
                CurrentHunger = tag.ContainsKey("currentHunger") ? tag.GetFloat("currentHunger") : 100f;
                MaxHunger = tag.ContainsKey("maxHunger") ? tag.GetFloat("maxHunger") : 100f;
                CurrentSanity = tag.ContainsKey("currentSanity") ? tag.GetFloat("currentSanity") : 100f;
                MaxSanity = tag.ContainsKey("maxSanity") ? tag.GetFloat("maxSanity") : 100f;
                CurrentStamina = tag.ContainsKey("currentStamina") ? tag.GetFloat("currentStamina") : 100f;
                MaxStamina = tag.ContainsKey("maxStamina") ? tag.GetFloat("maxStamina") : 100f;
                DashesUsed = tag.ContainsKey("dashesUsed") ? tag.GetInt("dashesUsed") : 0;
                DashResetTimer = tag.ContainsKey("dashResetTimer") ? tag.GetInt("dashResetTimer") : 0;
                DebugLog.Player("LoadData", $"Dados carregados - Classes: {ClassLevels.Count}, Habilidades: {UnlockedAbilities.Count}, Vitals: Hunger={CurrentHunger:F1}, Sanity={CurrentSanity:F1}, Stamina={CurrentStamina:F1}");
                DebugLog.Player("LoadData", $"Dicionário de classes após LoadData: {string.Join(", ", ClassLevels.Select(kv => kv.Key + ":" + kv.Value))}");
            }
            catch (System.Exception ex)
            {
                DebugLog.Error("Player", "LoadData", "Erro ao carregar dados do RPGPlayer", ex);
                InitializeClasses();
                DebugLog.Player("LoadData", $"Dicionário de classes após fallback: {string.Join(", ", ClassLevels.Select(kv => kv.Key + ":" + kv.Value))}");
            }
        }

        public override void OnEnterWorld()
        {
            DebugLog.Player("OnEnterWorld", $"Jogador '{Player.name}' entrou no mundo");
            DebugLog.Player("OnEnterWorld", $"Status inicial - Hunger: {CurrentHunger:F1}%, Sanity: {CurrentSanity:F1}%, Stamina: {CurrentStamina:F1}%");
            if (ClassLevels == null || ClassLevels.Count == 0)
            {
                DebugLog.Player("OnEnterWorld", "ClassLevels estava vazio, inicializando...");
                InitializeClasses();
            }
            DebugLog.Player("OnEnterWorld", $"Dicionário de classes ao entrar no mundo: {string.Join(", ", ClassLevels.Select(kv => kv.Key + ":" + kv.Value))}");
            
            // VERIFICAÇÃO ESPECÍFICA DO DASH AO ENTRAR NO MUNDO
            if (ClassLevels.ContainsKey("movement"))
            {
                float movementLevel = ClassLevels["movement"];
                int maxDashes = GetMaxDashes();
                DebugLog.Player("OnEnterWorld", $"DASH STATUS - Nível movimento: {movementLevel}, Max dashes: {maxDashes}, Dash disponível: {maxDashes > 0}");
                
                if (maxDashes > 0)
                {
                    DebugLog.Player("OnEnterWorld", "✅ DASH ATIVO - Jogador pode usar dash desde o nível 1 de movimento");
                    DebugLog.Player("OnEnterWorld", $"🎯 ITEM DE DASH SIMULADO - HasDashItem: {HasDashItem}");
                }
                else
                {
                    DebugLog.Error("Player", "OnEnterWorld", "❌ ERRO: Dash não disponível mesmo com nível de movimento!");
                }
            }
            else
            {
                DebugLog.Error("Player", "OnEnterWorld", "❌ ERRO: Classe 'movement' não encontrada!");
            }
        }

        public override void OnRespawn()
        {
            DebugLog.Player("OnRespawn", $"Jogador '{Player.name}' respawnou");
            // Restaurar vitals ao respawnar
            CurrentHunger = MaxHunger;
            CurrentSanity = MaxSanity;
            CurrentStamina = MaxStamina;
            DebugLog.Player("OnRespawn", "Vitals restaurados ao respawnar");
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Hook para quando o jogador atinge um NPC
            var item = Player.HeldItem;
            if (item != null && !item.IsAir)
            {
                string classType = GetDamageClassType(item.DamageType);
                float expGain = damageDone * 0.1f;
                
                // Bônus de XP para críticos e bosses
                if (hit.Crit) expGain *= 1.5f;
                if (target.boss) expGain *= 3f;
                
                // Verificar se é elite
                if (target.TryGetGlobalNPC<GlobalNPCs.BalancedNPC>(out var balancedNPC) && balancedNPC.IsElite)
                    expGain *= 2f;
                
                // XP para classe do jogador
                GainClassExp(classType, expGain);
                
                // === XP PARA O ITEM ===
                // Dar XP para o item usado
                if (item.TryGetGlobalItem<GlobalItems.ProgressiveItem>(out var progressiveItem))
                {
                    string reason = "usage";
                    if (hit.Crit) reason = "crit";
                    if (target.boss) reason = "boss";
                    if (balancedNPC?.IsElite == true) reason = "elite";
                    
                    float itemExpGain = damageDone * 0.2f; // Itens ganham mais XP que classes
                    progressiveItem.GainExperience(item, itemExpGain, reason);
                    
                    DebugLog.Gameplay("Player", "OnHitNPC", $"Item '{item.Name}' ganhou {itemExpGain:F1} XP (reason: {reason})");
                }
                
                DebugLog.Gameplay("Player", "OnHitNPC", $"Jogador atingiu '{target.FullName}' com '{item.Name}' - Dano: {damageDone}, XP ganho: {expGain:F1}, Classe: {classType}");
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Hook para quando um projétil do jogador atinge um NPC
            string classType = GetDamageClassType(proj.DamageType);
            float expGain = damageDone * 0.08f; // Menos XP que ataques diretos
            
            // Bônus de XP para críticos e bosses
            if (hit.Crit) expGain *= 1.5f;
            if (target.boss) expGain *= 3f;
            
            // Verificar se é elite
            if (target.TryGetGlobalNPC<GlobalNPCs.BalancedNPC>(out var balancedNPC) && balancedNPC.IsElite)
                expGain *= 2f;
            
            GainClassExp(classType, expGain);
            DebugLog.Gameplay("Player", "OnHitNPCWithProj", $"Projétil atingiu '{target.FullName}' - Dano: {damageDone}, XP ganho: {expGain:F1}, Classe: {classType}");
        }

        public void OnConsumeItem(Item item)
        {
            // Método para quando o jogador consome um item
            if (item.healLife > 0 || item.healMana > 0 || item.buffType > 0)
            {
                // Dar XP de "survival" por usar consumíveis
                GainClassExp("survival", 5f);
                
                // Se for comida, restaurar fome
                if (item.healLife > 0)
                {
                    float hungerRecovery = MathHelper.Clamp(item.healLife * 2f, 10f, 50f);
                    CurrentHunger = MathHelper.Clamp(CurrentHunger + hungerRecovery, 0f, MaxHunger);
                    DebugLog.Player("OnConsumeItem", $"Comida consumida: '{item.Name}' - Fome restaurada: {hungerRecovery:F1}, Fome atual: {CurrentHunger:F1}");
                }
                
                DebugLog.Gameplay("Player", "OnConsumeItem", $"Item consumido: '{item.Name}' - XP de survival ganho: 5");
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            // Hook para quando o jogador recebe dano
            float defenseExp = info.Damage * 0.3f;
            GainClassExp("defense", defenseExp);
            
            // === XP PARA ARMADURAS ===
            // Dar XP para armaduras equipadas quando recebe dano
            for (int i = 0; i < Player.armor.Length; i++)
            {
                var armorItem = Player.armor[i];
                if (armorItem != null && !armorItem.IsAir && armorItem.defense > 0)
                {
                    if (armorItem.TryGetGlobalItem<GlobalItems.ProgressiveItem>(out var progressiveItem))
                    {
                        float armorExpGain = info.Damage * 0.1f; // Armaduras ganham XP por receber dano
                        progressiveItem.GainExperience(armorItem, armorExpGain, "defense");
                        
                        DebugLog.Gameplay("Player", "OnHurt", $"Armadura '{armorItem.Name}' ganhou {armorExpGain:F1} XP por defesa");
                    }
                }
            }
            
            // Reduzir sanidade quando recebe dano
            float sanityLoss = info.Damage * 0.1f;
            CurrentSanity = MathHelper.Clamp(CurrentSanity - sanityLoss, 0f, MaxSanity);
            
            // Entrar em combate
            CombatTimer = 300; // 5 segundos em 60 FPS
            
            DebugLog.Gameplay("Player", "OnHurt", $"Jogador recebeu dano: {info.Damage} - XP de defesa: {defenseExp:F1}, Sanidade perdida: {sanityLoss:F1}");
        }

        public override void OnMissingMana(Item item, int neededMana)
        {
            // Hook para quando o jogador não tem mana suficiente
            DebugLog.Player("OnMissingMana", $"Mana insuficiente para usar '{item.Name}' - Necessário: {neededMana}, Atual: {Player.statMana}");
        }

        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            // Bônus de pesca baseado no nível da classe
            float fishingClassLevel = GetClassLevel("fishing");
            fishingLevel += fishingClassLevel * 0.5f;
            
            DebugLog.Player("GetFishingLevel", $"Nível de pesca modificado - Classe: {fishingClassLevel:F1}, Bônus: {fishingClassLevel * 0.5f:F1}, Total: {fishingLevel:F1}");
        }

        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            // Hook para quando o jogador pesca algo
            if (itemDrop > 0)
            {
                GainClassExp("fishing", 10f);
                DebugLog.Gameplay("Player", "CatchFish", $"Peixe pescado - Item: {itemDrop}, XP de pesca ganho: 10");
            }
        }

        // Método auxiliar para determinar o tipo de classe baseado no tipo de dano
        private string GetDamageClassType(DamageClass damageType)
        {
            if (damageType == DamageClass.Melee) return "melee";
            if (damageType == DamageClass.Ranged) return "ranged";
            if (damageType == DamageClass.Magic) return "magic";
            if (damageType == DamageClass.Summon) return "summoner";
            return "melee"; // Fallback
        }

        public override void PreUpdate()
        {
            // Atualizar cooldowns e reset
            if (DashCooldown > 0) DashCooldown--;
            if (DashResetTimer > 0) {
                DashResetTimer--;
                if (DashResetTimer == 0) DashesUsed = 0;
            }

            // DETECÇÃO MELHORADA DE DUPLO-TOQUE
            // Usar uma abordagem mais robusta baseada na documentação do tModLoader
            
            // Detectar duplo-toque horizontal
            bool dashLeft = false;
            bool dashRight = false;
            
            // Verificar se o jogador pressionou e soltou a tecla esquerda
            if (Player.controlLeft && Player.releaseLeft)
            {
                // Verificar se o timer de duplo-toque está ativo
                if (Player.doubleTapCardinalTimer[2] > 0 && Player.doubleTapCardinalTimer[2] < 15)
                {
                    dashLeft = true;
                    DebugLog.Player("PreUpdate", $"🎯 Duplo-toque ESQUERDA detectado! Timer: {Player.doubleTapCardinalTimer[2]}");
                }
            }
            
            // Verificar se o jogador pressionou e soltou a tecla direita
            if (Player.controlRight && Player.releaseRight)
            {
                // Verificar se o timer de duplo-toque está ativo
                if (Player.doubleTapCardinalTimer[3] > 0 && Player.doubleTapCardinalTimer[3] < 15)
                {
                    dashRight = true;
                    DebugLog.Player("PreUpdate", $"🎯 Duplo-toque DIREITA detectado! Timer: {Player.doubleTapCardinalTimer[3]}");
                }
            }
            
            int dir = 0;
            if (dashLeft) dir = -1;
            if (dashRight) dir = 1;

            // Detectar duplo-toque vertical (liberado a partir do nível 50 de movimento)
            bool allowVerticalDash = GetClassLevel("movement") >= 50f;
            bool dashUp = false;
            bool dashDown = false;
            
            if (allowVerticalDash)
            {
                if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[0] > 0 && Player.doubleTapCardinalTimer[0] < 15)
                {
                    dashUp = true;
                    DebugLog.Player("PreUpdate", $"🎯 Duplo-toque CIMA detectado! Timer: {Player.doubleTapCardinalTimer[0]}");
                }
                
                if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[1] > 0 && Player.doubleTapCardinalTimer[1] < 15)
                {
                    dashDown = true;
                    DebugLog.Player("PreUpdate", $"🎯 Duplo-toque BAIXO detectado! Timer: {Player.doubleTapCardinalTimer[1]}");
                }
            }
            
            int vdir = 0;
            if (dashUp) vdir = -1;
            if (dashDown) vdir = 1;

            // Log de debug para detecção de duplo-toque (a cada 30 frames para não spam)
            if (Main.GameUpdateCount % 30 == 0)
            {
                DebugLog.Player("PreUpdate", $"📊 Status dos timers - Left: {Player.doubleTapCardinalTimer[2]}, Right: {Player.doubleTapCardinalTimer[3]}, Up: {Player.doubleTapCardinalTimer[0]}, Down: {Player.doubleTapCardinalTimer[1]}");
                DebugLog.Player("PreUpdate", $"🎮 Controls - Left: {Player.controlLeft}, Right: {Player.controlRight}, Up: {Player.controlUp}, Down: {Player.controlDown}");
                DebugLog.Player("PreUpdate", $"🔓 Release - Left: {Player.releaseLeft}, Right: {Player.releaseRight}, Up: {Player.releaseUp}, Down: {Player.releaseDown}");
            }

            // Dash horizontal
            if (dir != 0 && CanDash(dir)) {
                DebugLog.Player("PreUpdate", $"🚀 Executando dash horizontal - Direção: {dir}, CanDash: {CanDash(dir)}");
                DoDash(dir, 0);
            }
            // Dash vertical
            else if (vdir != 0 && CanDash(0)) {
                DebugLog.Player("PreUpdate", $"🚀 Executando dash vertical - Direção: {vdir}, CanDash: {CanDash(0)}");
                DoDash(0, vdir);
            }
        }
    }
}
 