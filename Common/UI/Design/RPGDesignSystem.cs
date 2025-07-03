using Microsoft.Xna.Framework;
using Terraria;

namespace Wolfgodrpg.Common.UI.Design
{
    /// <summary>
    /// Sistema de design moderno para UI do WolfgodRPG.
    /// Baseado nas melhores práticas do tModLoader e design systems modernos.
    /// </summary>
    public static class RPGDesignSystem
    {
        // === PALETA DE CORES PRINCIPAL ===
        public static class Colors
        {
            // Cores primárias
            public static readonly Color Primary = new Color(70, 120, 200);      // Azul principal
            public static readonly Color PrimaryDark = new Color(50, 90, 150);   // Azul escuro
            public static readonly Color PrimaryLight = new Color(100, 150, 220); // Azul claro
            
            // Cores secundárias
            public static readonly Color Secondary = new Color(255, 193, 7);     // Dourado
            public static readonly Color SecondaryDark = new Color(200, 150, 0); // Dourado escuro
            public static readonly Color SecondaryLight = new Color(255, 220, 100); // Dourado claro
            
            // Cores de estado
            public static readonly Color Success = new Color(76, 175, 80);       // Verde sucesso
            public static readonly Color Warning = new Color(255, 152, 0);       // Laranja aviso
            public static readonly Color Error = new Color(244, 67, 54);         // Vermelho erro
            public static readonly Color Info = new Color(33, 150, 243);         // Azul info
            
            // Cores neutras
            public static readonly Color Background = new Color(30, 30, 30);     // Fundo escuro
            public static readonly Color Surface = new Color(45, 45, 45);        // Superfície
            public static readonly Color Border = new Color(60, 60, 60);         // Borda
            public static readonly Color Text = new Color(255, 255, 255);        // Texto branco
            public static readonly Color TextSecondary = new Color(180, 180, 180); // Texto secundário
            public static readonly Color TextMuted = new Color(120, 120, 120);   // Texto mudo
            
            // Cores de classe (para notificações e elementos específicos)
            public static readonly Color Warrior = new Color(220, 53, 69);       // Vermelho guerreiro
            public static readonly Color Archer = new Color(40, 167, 69);        // Verde arqueiro
            public static readonly Color Mage = new Color(111, 66, 193);         // Roxo mago
            public static readonly Color Summoner = new Color(255, 123, 0);      // Laranja invocador
            public static readonly Color Acrobat = new Color(23, 162, 184);      // Ciano acrobata
            public static readonly Color Explorer = new Color(255, 193, 7);      // Amarelo explorador
            public static readonly Color Engineer = new Color(108, 117, 125);    // Cinza engenheiro
            public static readonly Color Survivalist = new Color(165, 42, 42);   // Marrom sobrevivente
            public static readonly Color Blacksmith = new Color(220, 53, 69);    // Vermelho ferreiro
            public static readonly Color Alchemist = new Color(40, 167, 69);     // Verde alquimista
            public static readonly Color Mystic = new Color(232, 62, 140);       // Rosa místico
        }

        // === TIPOGRAFIA ===
        public static class Typography
        {
            // Tamanhos de fonte
            public const float Display = 1.4f;      // Títulos principais
            public const float Heading = 1.2f;      // Cabeçalhos
            public const float Title = 1.0f;        // Títulos de seção
            public const float Body = 0.85f;        // Texto principal
            public const float Caption = 0.75f;     // Texto pequeno
            public const float Small = 0.7f;        // Texto muito pequeno
            
            // Pesos de fonte (usando bool para bold)
            public const bool Bold = true;
            public const bool Regular = false;
        }

        // === ESPAÇAMENTOS ===
        public static class Spacing
        {
            // Espaçamentos verticais
            public const float XS = 4f;     // Muito pequeno
            public const float S = 8f;      // Pequeno
            public const float M = 12f;     // Médio
            public const float L = 16f;     // Grande
            public const float XL = 24f;    // Muito grande
            public const float XXL = 32f;   // Extra grande
            
            // Padding padrão
            public const float PaddingXS = 4f;
            public const float PaddingS = 8f;
            public const float PaddingM = 12f;
            public const float PaddingL = 16f;
            public const float PaddingXL = 24f;
        }

        // === BORDAS E RAIO ===
        public static class Borders
        {
            public const float RadiusS = 4f;    // Raio pequeno
            public const float RadiusM = 8f;    // Raio médio
            public const float RadiusL = 12f;   // Raio grande
            
            public const float BorderS = 1f;    // Borda fina
            public const float BorderM = 2f;    // Borda média
            public const float BorderL = 3f;    // Borda grossa
        }

        // === ANIMAÇÕES ===
        public static class Animation
        {
            public const float Fast = 0.15f;      // Animação rápida
            public const float Normal = 0.25f;    // Animação normal
            public const float Slow = 0.35f;      // Animação lenta
            
            public const float HoverScale = 1.05f; // Escala no hover
            public const float ClickScale = 0.95f; // Escala no click
        }

        // === MÉTODOS UTILITÁRIOS ===
        
        /// <summary>
        /// Obtém a cor de uma classe específica.
        /// </summary>
        public static Color GetClassColor(string className)
        {
            return className switch
            {
                "warrior" => Colors.Warrior,
                "archer" => Colors.Archer,
                "mage" => Colors.Mage,
                "summoner" => Colors.Summoner,
                "acrobat" => Colors.Acrobat,
                "explorer" => Colors.Explorer,
                "engineer" => Colors.Engineer,
                "survivalist" => Colors.Survivalist,
                "blacksmith" => Colors.Blacksmith,
                "alchemist" => Colors.Alchemist,
                "mystic" => Colors.Mystic,
                _ => Colors.Text
            };
        }

        /// <summary>
        /// Obtém a cor baseada no valor de um vital (fome, sanidade, stamina).
        /// </summary>
        public static Color GetVitalColor(float value)
        {
            if (value > 70f) return Colors.Success;
            if (value > 30f) return Colors.Warning;
            return Colors.Error;
        }

        /// <summary>
        /// Obtém a cor baseada no progresso (0-100%).
        /// </summary>
        public static Color GetProgressColor(float progress)
        {
            if (progress >= 80f) return Colors.Success;
            if (progress >= 50f) return Colors.Warning;
            return Colors.Error;
        }

        /// <summary>
        /// Cria uma cor com transparência.
        /// </summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.R, color.G, color.B, (int)(255 * alpha));
        }

        /// <summary>
        /// Cria uma cor mais clara.
        /// </summary>
        public static Color Lighten(Color color, float factor = 0.2f)
        {
            return new Color(
                (int)(color.R + (255 - color.R) * factor),
                (int)(color.G + (255 - color.G) * factor),
                (int)(color.B + (255 - color.B) * factor),
                color.A
            );
        }

        /// <summary>
        /// Cria uma cor mais escura.
        /// </summary>
        public static Color Darken(Color color, float factor = 0.2f)
        {
            return new Color(
                (int)(color.R * (1 - factor)),
                (int)(color.G * (1 - factor)),
                (int)(color.B * (1 - factor)),
                color.A
            );
        }

        /// <summary>
        /// Interpola entre duas cores.
        /// </summary>
        public static Color Lerp(Color color1, Color color2, float t)
        {
            return new Color(
                (int)(color1.R + (color2.R - color1.R) * t),
                (int)(color1.G + (color2.G - color1.G) * t),
                (int)(color1.B + (color2.B - color1.B) * t),
                (int)(color1.A + (color2.A - color1.A) * t)
            );
        }
    }
}
