using System;
using System.Collections.Generic;
using System.IO;

namespace ClownJumper;

/// <summary>
/// Lê e escreve o save do modo história em um arquivo texto simples no
/// formato "chave=valor", uma entrada por linha. O arquivo fica na pasta
/// de dados do usuário (a mesma usada por outros jogos/apps do sistema),
/// então ele sobrevive a reinstalações do jogo e funciona em qualquer
/// sistema operacional suportado.
/// </summary>
public static class SoloSaveManager
{
    private const string SaveFileName = "save.txt";

    /// <summary>
    /// Caminho completo do arquivo de save, criando a pasta se necessário.
    /// Ex.: %APPDATA%/ClownJumper/save.txt no Windows,
    /// ~/.config/ClownJumper/save.txt no Linux/Mac.
    /// </summary>
    public static string GetSaveFilePath()
    {
        string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string gameFolder = Path.Combine(baseFolder, "ClownJumper");

        Directory.CreateDirectory(gameFolder);

        return Path.Combine(gameFolder, SaveFileName);
    }

    /// <summary>
    /// Verifica se já existe um save salvo em disco.
    /// </summary>
    public static bool SaveExists()
    {
        return File.Exists(GetSaveFilePath());
    }

    /// <summary>
    /// Carrega o save existente. Se não existir arquivo, ou se ele estiver
    /// corrompido/incompleto, cria e retorna um save novo com os valores
    /// iniciais (sem gravar em disco automaticamente).
    /// </summary>
    public static SoloSaveData Load()
    {
        string path = GetSaveFilePath();

        if (!File.Exists(path))
        {
            return SoloSaveData.CreateNew();
        }

        try
        {
            var values = new Dictionary<string, string>();

            foreach (var line in File.ReadAllLines(path))
            {
                string trimmed = line.Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#"))
                    continue;

                int separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                string key = trimmed.Substring(0, separatorIndex).Trim();
                string value = trimmed.Substring(separatorIndex + 1).Trim();

                values[key] = value;
            }

            var data = SoloSaveData.CreateNew();

            data.Money = ReadInt(values, "Money", data.Money);
            data.LivesUpgradeLevel = ReadInt(values, "LivesUpgradeLevel", data.LivesUpgradeLevel);
            data.LuckUpgradeLevel = ReadInt(values, "LuckUpgradeLevel", data.LuckUpgradeLevel);
            data.QuantityUpgradeLevel = ReadInt(values, "QuantityUpgradeLevel", data.QuantityUpgradeLevel);
            data.DurationUpgradeLevel = ReadInt(values, "DurationUpgradeLevel", data.DurationUpgradeLevel);
            data.CurrentPhase = ReadInt(values, "CurrentPhase", data.CurrentPhase);

            return data;
        }
        catch (IOException)
        {
            return SoloSaveData.CreateNew();
        }
        catch (UnauthorizedAccessException)
        {
            return SoloSaveData.CreateNew();
        }
    }

    /// <summary>
    /// Grava o save atual em disco, sobrescrevendo o arquivo existente.
    /// </summary>
    public static void Save(SoloSaveData data)
    {
        string path = GetSaveFilePath();

        var lines = new List<string>
        {
            $"Money={data.Money}",
            $"LivesUpgradeLevel={data.LivesUpgradeLevel}",
            $"LuckUpgradeLevel={data.LuckUpgradeLevel}",
            $"QuantityUpgradeLevel={data.QuantityUpgradeLevel}",
            $"DurationUpgradeLevel={data.DurationUpgradeLevel}",
            $"CurrentPhase={data.CurrentPhase}",
        };

        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// Cria um save novo (valores iniciais) e já grava em disco,
    /// sobrescrevendo qualquer save anterior. Usado pela opção "Novo Jogo".
    /// </summary>
    public static SoloSaveData CreateAndSaveNew()
    {
        var data = SoloSaveData.CreateNew();
        Save(data);
        return data;
    }

    private static int ReadInt(Dictionary<string, string> values, string key, int defaultValue)
    {
        if (values.TryGetValue(key, out string raw) && int.TryParse(raw, out int parsed))
        {
            return parsed;
        }

        return defaultValue;
    }
}
