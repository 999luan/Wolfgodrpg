using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;

namespace Wolfgodrpg.Common.Systems
{
    /// <summary>
    /// Sistema modular para gerenciar vitais do jogador (Fome, Sanidade, Stamina)
    /// </summary>
    public class VitalsSystem
    {
        // === CONSTANTES ===
        public const float MAX_HUNGER = 100f;
        public const float MAX_SANITY = 100f;
        public const float MAX_STAMINA = 100f;
        
        public const float HUNGER_REGEN_RATE = 0.5f;
        public const float SANITY_REGEN_RATE = 0.3f;
        public const float STAMINA_REGEN_RATE = 0.7f;

        // === EVENTOS ===
        public event Action OnHungerChanged;
        public event Action OnSanityChanged;
        public event Action OnStaminaChanged;
        public event Action OnVitalsCritical; // Quando algum vital fica muito baixo

        // === PROPRIEDADES PRIVADAS ===
        private float currentHunger;
        private float currentSanity;
        private float currentStamina;

        // === PROPRIEDADES PÚBLICAS COM VALIDAÇÃO ===
        public float CurrentHunger
        {
            get => currentHunger;
            set
            {
                float oldValue = currentHunger;
                currentHunger = MathHelper.Clamp(value, 0f, MAX_HUNGER);
                
                if (Math.Abs(oldValue - currentHunger) > 0.01f)
                {
                    OnHungerChanged?.Invoke();
                    CheckCriticalVitals();
                }
            }
        }

        public float CurrentSanity
        {
            get => currentSanity;
            set
            {
                float oldValue = currentSanity;
                currentSanity = MathHelper.Clamp(value, 0f, MAX_SANITY);
                
                if (Math.Abs(oldValue - currentSanity) > 0.01f)
                {
                    OnSanityChanged?.Invoke();
                    CheckCriticalVitals();
                }
            }
        }

        public float CurrentStamina
        {
            get => currentStamina;
            set
            {
                float oldValue = currentStamina;
                currentStamina = MathHelper.Clamp(value, 0f, MAX_STAMINA);
                
                if (Math.Abs(oldValue - currentStamina) > 0.01f)
                {
                    OnStaminaChanged?.Invoke();
                    CheckCriticalVitals();
                }
            }
        }

        // === CONSTRUTOR ===
        public VitalsSystem()
        {
            // Inicializar com valores máximos
            currentHunger = MAX_HUNGER;
            currentSanity = MAX_SANITY;
            currentStamina = MAX_STAMINA;
        }

        // === MÉTODOS DE CONSUMO ===
        /// <summary>
        /// Consome fome e retorna se foi bem-sucedido
        /// </summary>
        public bool ConsumeHunger(float amount)
        {
            if (CurrentHunger >= amount)
            {
                CurrentHunger -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consome sanidade e retorna se foi bem-sucedido
        /// </summary>
        public bool ConsumeSanity(float amount)
        {
            if (CurrentSanity >= amount)
            {
                CurrentSanity -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consome stamina e retorna se foi bem-sucedido
        /// </summary>
        public bool ConsumeStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                CurrentStamina -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Consome stamina em porcentagem do máximo
        /// </summary>
        public bool ConsumeStaminaPercent(float percent)
        {
            float amount = MAX_STAMINA * (percent / 100f);
            return ConsumeStamina(amount);
        }

        // === MÉTODOS DE REGENERAÇÃO ===
        /// <summary>
        /// Regenera fome baseado no tempo
        /// </summary>
        public void RegenerateHunger(float deltaTime)
        {
            if (CurrentHunger < MAX_HUNGER)
            {
                CurrentHunger += HUNGER_REGEN_RATE * deltaTime;
            }
        }

        /// <summary>
        /// Regenera sanidade baseado no tempo
        /// </summary>
        public void RegenerateSanity(float deltaTime)
        {
            if (CurrentSanity < MAX_SANITY)
            {
                CurrentSanity += SANITY_REGEN_RATE * deltaTime;
            }
        }

        /// <summary>
        /// Regenera stamina baseado no tempo
        /// </summary>
        public void RegenerateStamina(float deltaTime)
        {
            if (CurrentStamina < MAX_STAMINA)
            {
                CurrentStamina += STAMINA_REGEN_RATE * deltaTime;
            }
        }

        // === MÉTODOS DE VERIFICAÇÃO ===
        /// <summary>
        /// Verifica se algum vital está crítico (abaixo de 20%)
        /// </summary>
        private void CheckCriticalVitals()
        {
            bool isCritical = CurrentHunger < MAX_HUNGER * 0.2f ||
                             CurrentSanity < MAX_SANITY * 0.2f ||
                             CurrentStamina < MAX_STAMINA * 0.2f;

            if (isCritical)
            {
                OnVitalsCritical?.Invoke();
            }
        }

        /// <summary>
        /// Verifica se o jogador tem stamina suficiente para uma ação
        /// </summary>
        public bool HasStaminaForAction(float requiredStamina)
        {
            return CurrentStamina >= requiredStamina;
        }

        /// <summary>
        /// Verifica se o jogador tem fome suficiente para uma ação
        /// </summary>
        public bool HasHungerForAction(float requiredHunger)
        {
            return CurrentHunger >= requiredHunger;
        }

        /// <summary>
        /// Verifica se o jogador tem sanidade suficiente para uma ação
        /// </summary>
        public bool HasSanityForAction(float requiredSanity)
        {
            return CurrentSanity >= requiredSanity;
        }

        // === MÉTODOS DE ESTADO ===
        /// <summary>
        /// Retorna o estado geral dos vitais (0-100%)
        /// </summary>
        public float GetOverallVitality()
        {
            return (CurrentHunger + CurrentSanity + CurrentStamina) / 3f;
        }

        /// <summary>
        /// Retorna se todos os vitais estão em níveis seguros (>50%)
        /// </summary>
        public bool AreVitalsHealthy()
        {
            return CurrentHunger > MAX_HUNGER * 0.5f &&
                   CurrentSanity > MAX_SANITY * 0.5f &&
                   CurrentStamina > MAX_STAMINA * 0.5f;
        }

        /// <summary>
        /// Retorna se algum vital está crítico (<20%)
        /// </summary>
        public bool IsAnyVitalCritical()
        {
            return CurrentHunger < MAX_HUNGER * 0.2f ||
                   CurrentSanity < MAX_SANITY * 0.2f ||
                   CurrentStamina < MAX_STAMINA * 0.2f;
        }

        // === MÉTODOS DE RESET ===
        /// <summary>
        /// Reseta todos os vitais para o máximo
        /// </summary>
        public void ResetToMax()
        {
            CurrentHunger = MAX_HUNGER;
            CurrentSanity = MAX_SANITY;
            CurrentStamina = MAX_STAMINA;
        }

        /// <summary>
        /// Reseta vitais para valores específicos
        /// </summary>
        public void ResetToValues(float hunger, float sanity, float stamina)
        {
            CurrentHunger = hunger;
            CurrentSanity = sanity;
            CurrentStamina = stamina;
        }

        // === MÉTODOS DE DEBUG ===
        /// <summary>
        /// Retorna string formatada com status dos vitais
        /// </summary>
        public string GetVitalsStatus()
        {
            return $"Hunger: {CurrentHunger:F1}/{MAX_HUNGER} | " +
                   $"Sanity: {CurrentSanity:F1}/{MAX_SANITY} | " +
                   $"Stamina: {CurrentStamina:F1}/{MAX_STAMINA}";
        }

        /// <summary>
        /// Loga o status atual dos vitais
        /// </summary>
        public void LogVitalsStatus()
        {
            Main.NewText($"Vitals Status: {GetVitalsStatus()}", Color.Yellow);
        }
    }
}
