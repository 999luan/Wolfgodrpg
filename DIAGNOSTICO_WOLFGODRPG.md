# DIAGNÓSTICO TÉCNICO - WOLFGOD RPG MOD
**Data:** 2024-12-28  
**Versão Atual tModLoader:** 1.4.4+ (2024)  
**Versão do Mod:** 0.7  
**Novo Design:** Sistema de 3 Eixos + Afixos + Proficiência de Armaduras

## 🎯 NOVA VISÃO DO PROJETO (2024)

### **Sistema de Progressão Redesenhado:**
1. **Nível do Jogador (Geral)** - XP por marcos importantes
2. **Níveis de Classe (Ação)** - 10 classes divididas em:
   - **Combate:** Guerreiro, Arqueiro, Mago, Invocador
   - **Utilidade:** Acrobata, Explorador, Engenheiro, Sobrevivencialista, Ferreiro, Alquimista
3. **Proficiência de Equipamentos** - Armas + **Armaduras** (Pesada/Leve/Mágicas)

### **Novos Recursos Planejados:**
- **Sistema de Afixos** em itens (bônus aleatórios)
- **UI de 3 Abas** (Atributos, Classes, Proficiências)
- **Cálculo integrado** de todas as fontes de bônus

## 🚨 PROBLEMAS CRÍTICOS (vs. Nova Visão)

### 1. USO DE `AssetRequestMode.ImmediateLoad` (DESATUALIZADO)
**Arquivo:** `Common/UI/RPGTabButton.cs` (linhas 39-40)  
**Problema:** Uso de `AssetRequestMode.ImmediateLoad` que é desencorajado no tModLoader atual  
**Impacto:** Performance ruim, tempos de carregamento lentos  
**Solução:**
```csharp
// ❌ ATUAL (PROBLEMÁTICO)
_normalTexture = ModContent.Request<Texture2D>(_normalTexturePath, AssetRequestMode.ImmediateLoad).Value;

// ✅ MODERNO (RECOMENDADO)
private Asset<Texture2D> _normalTextureAsset;
private Asset<Texture2D> _selectedTextureAsset;

// Em SetDefaults/Initialize:
_normalTextureAsset = ModContent.Request<Texture2D>(_normalTexturePath);
_selectedTextureAsset = ModContent.Request<Texture2D>(_selectedTexturePath);

// Ao usar:
if (_normalTextureAsset.IsLoaded)
    spriteBatch.Draw(_normalTextureAsset.Value, position, Color.White);
```

### 2. LOCALIZAÇÃO DESATUALIZADA (CRÍTICO)
**Problema:** Sistema de localização ainda usa padrões antigos  
**Impacto:** Incompatibilidade com tModLoader 2024+  
**Arquivos Afetados:** 
- `Localization/en-US_Mods.Wolfgodrpg.hjson`
- Múltiplas entradas duplicadas e mal organizadas  

**Soluções Necessárias:**
1. Reorganizar arquivos de localização seguindo padrão `Mods.{ModName}.{Category}.{ContentName}.{DataName}`
2. Remover entradas duplicadas (ex: Config com Label/Tooltip vazios)
3. Consolidar traduções hardcoded no código

### 3. BUILD.TXT INCOMPLETO
**Arquivo:** `build.txt`  
**Problema:** Faltam configurações essenciais para 2024  
**Atual:**
```
displayName = WolfGod RPG
author = WolfGod
version = 0.7
side = Both
```

**Recomendado:**
```
displayName = WolfGod RPG
author = WolfGod
version = 0.7.1
side = Both
homepage = https://github.com/[user]/Wolfgodrpg
description = Sistema RPG completo para Terraria com classes, níveis, fome e sanidade
includeSource = true
buildIgnore = *.csproj.user, *.suo, bin/, obj/, .vs/
```

### 4. **SISTEMA DE AFIXOS NÃO IMPLEMENTADO** ⭐ NOVO
**Problema:** O novo sistema de afixos em itens planejado não existe no código atual  
**Impacto:** Funcionalidade principal do redesign não implementada  
**Necessário:**
- TagCompound para armazenar afixos em itens
- Geração de afixos aleatórios em OnCreate
- ModifyTooltips para exibir afixos
- Sistema de aplicação de bônus dos afixos

### 5. **PROFICIÊNCIA DE ARMADURAS FALTANDO** ⭐ NOVO  
**Problema:** Só existe proficiência de armas, falta para armaduras  
**Impacto:** 1/3 do sistema de proficiência não implementado  
**Necessário:**
- Dicionários `armorProficiencyLevels` e `armorProficiencyExperience`
- Hooks OnHitByNPC/OnHitByProjectile para XP de armadura
- 3 tipos: Pesada, Leve, Vestes Mágicas

### 6. **CLASSES DESATUALIZADAS vs. NOVO DESIGN** ⭐ NOVO
**Problema:** Classes atuais não correspondem ao novo design de 10 classes  
**Impacto:** Sistema não condiz com a nova visão  
**Atual:** Sistema básico  
**Novo:** Combate (4) + Utilidade (6) com habilidades específicas

## ⚠️ PROBLEMAS IMPORTANTES (Prioridade Média)

### 7. ORGANIZAÇÃO DE CÓDIGO INCONSISTENTE
**Problemas:**
- Classes de UI espalhadas sem padrão consistente
- Sistemas misturados (RPG, Debug, Actions)
- Falta de separação clara entre client/server code

**Reorganização Sugerida:**
```
Common/
├── Systems/
│   ├── RPGSystem.cs (core)
│   ├── VitalsSystem.cs
│   └── ConfigSystem.cs
├── Players/
│   └── RPGPlayer.cs
├── UI/
│   ├── Base/
│   ├── Menus/
│   └── HUD/
└── GlobalClasses/
```

### 8. USO EXCESSIVO DE `Main.LocalPlayer`
**Arquivos Afetados:** Múltiplos (15+ ocorrências)  
**Problema:** Verificações inadequadas podem causar NullReference  
**Soluções:**
- Sempre verificar `Main.LocalPlayer != null && Main.LocalPlayer.active`
- Considerar usar `ModContent.GetInstance<RPGPlayer>()` quando apropriado
- Implementar checks seguros em todos os acessos

### 9. SISTEMA DE NETWORK INCOMPLETO
**Problema:** Falta implementação adequada de sincronização multiplayer  
**Impacto:** Possíveis dessincs em multiplayer  
**Necessário:**
- Implementar `SendClientChanges` em RPGPlayer
- Adicionar network sync para vitals system
- Verificar sincronização de configs

## 📋 MELHORIAS TÉCNICAS (Prioridade Baixa)

### 10. PERFORMANCE E OTIMIZAÇÃO
**Sugestões:**
- Implementar caching para cálculos de stats
- Otimizar loops de UI updates
- Usar object pooling para elementos UI reutilizáveis

### 11. PADRÕES DE CÓDIGO MODERNOS
**Atualizações Recomendadas:**
- Migrar para C# 8+ features (pattern matching, null coalescing)
- Implementar `IDisposable` para recursos UI
- Usar `readonly` para campos imutáveis

### 12. SISTEMA DE DEBUGGING
**Melhorias:**
- Consolidar sistema DebugLog atual
- Implementar logs categorizados
- Adicionar debug UI toggle

## 🔧 PLANO DE CORREÇÃO RECOMENDADO

### FASE 1: Correções Críticas (1-2 dias)
1. ✅ **Corrigir AssetRequestMode.ImmediateLoad**
2. ✅ **Reorganizar sistema de localização**
3. ✅ **Atualizar build.txt**

### FASE 2: Estabilização (3-5 dias)
1. ✅ **Reorganizar estrutura de pastas**
2. ✅ **Implementar network sync adequado**
3. ✅ **Padronizar verificações Main.LocalPlayer**

### FASE 3: Polimento (1-2 dias)
1. ✅ **Otimizações de performance**
2. ✅ **Implementar melhorias de código**
3. ✅ **Testes finais e debugging**

## 📊 ESTATÍSTICAS DO PROJETO

- **Total de Classes:** ~25-30
- **Linhas de Código:** ~3000-4000
- **Sistemas Principais:** 7 (Classes, Vitals, UI, Config, etc.)
- **Problemas Críticos:** 3
- **Problemas Importantes:** 4
- **Melhorias Sugeridas:** 3

## 🏆 PONTOS POSITIVOS ENCONTRADOS

1. ✅ **Estrutura modular bem definida**
2. ✅ **Sistema de classes complexo e funcional**
3. ✅ **UI personalizada implementada**
4. ✅ **Sistema de configuração robusto**
5. ✅ **Boa cobertura de funcionalidades RPG**

## 📚 RECURSOS RECOMENDADOS

- [tModLoader Wiki - Update Migration Guide](https://github.com/tModLoader/tModLoader/wiki/Update-Migration-Guide)
- [Assets Guide](https://github.com/tModLoader/tModLoader/wiki/Assets)
- [Localization Guide](https://github.com/tModLoader/tModLoader/wiki/Localization)
- [ExampleMod Reference](https://github.com/tModLoader/tModLoader/tree/1.4/ExampleMod)

## 🎯 PRÓXIMOS PASSOS

1. **Implementar correções da Fase 1** para compatibilidade imediata
2. **Testar em tModLoader 2024+** após cada correção
3. **Validar multiplayer** após implementar network sync
4. **Considerar publicação** após Fase 2 completa

---
**Nota:** Este diagnóstico foi baseado na análise do código atual e nas melhores práticas do tModLoader 2024. Recomenda-se implementar as correções em ordem de prioridade. 