using System.Reflection;
using System.Text.Json;
using System.Linq;
using System.Collections.Generic;
using System;

namespace consoleApp
{
    public struct RevenuInfo { public int Annee; public int Revenu; }

    public static class RessourcesChargement
    {
        public static List<(Formulaires.FormulaireNouveauCompte Form, List<RevenuInfo> Revenus)> ChargerPremiersDepuisRessourceAvecAgeEtRevenus()
        {
            var result = new List<(Formulaires.FormulaireNouveauCompte, List<RevenuInfo>)>();
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resourceName = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith("premiers.json"));
                if (resourceName == null) return result;

                using var stream = asm.GetManifestResourceStream(resourceName);
                if (stream == null) return result;

                using var doc = JsonDocument.Parse(stream);
                if (doc.RootElement.ValueKind != JsonValueKind.Array) return result;

                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (var prop in element.EnumerateObject())
                    {
                        try
                        {
                            if (prop.Value.ValueKind == JsonValueKind.String)
                                map[prop.Name] = prop.Value.GetString();
                            else
                                map[prop.Name] = prop.Value.ToString();
                        }
                        catch { }
                    }

                    map.TryGetValue("Nom", out var nom);
                    map.TryGetValue("NAS", out var nas);
                    map.TryGetValue("MotDePasse", out var mdp);

                    var revenusList = new List<RevenuInfo>();
                    if (element.TryGetProperty("Revenus", out var revenusProp) && revenusProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var r in revenusProp.EnumerateArray())
                        {
                            try
                            {
                                int annee = 0; int revenu = 0;
                                if (r.TryGetProperty("Annee", out var anneeProp) && anneeProp.ValueKind == JsonValueKind.Number && anneeProp.TryGetInt32(out var ay)) annee = ay;
                                else if (r.TryGetProperty("Annee", out var anneeProp2) && anneeProp2.ValueKind == JsonValueKind.String && int.TryParse(anneeProp2.GetString(), out var ay2)) annee = ay2;

                                if (r.TryGetProperty("Revenu", out var revProp) && revProp.ValueKind == JsonValueKind.Number && revProp.TryGetInt32(out var rv)) revenu = rv;
                                else if (r.TryGetProperty("Revenu", out var revProp2) && revProp2.ValueKind == JsonValueKind.String && int.TryParse(revProp2.GetString(), out var rv2)) revenu = rv2;

                                if (annee != 0)
                                {
                                    revenusList.Add(new RevenuInfo { Annee = annee, Revenu = revenu });
                                }
                            }
                            catch { }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(nom)) continue;

                    var form = new Formulaires.FormulaireNouveauCompte
                    {
                        Nom = nom,
                        NAS = nas ?? string.Empty,
                        MotDePasse = mdp ?? string.Empty,
                        MotDePasseConfirmation = mdp ?? string.Empty
                    };

                    result.Add((form, revenusList));
                }
            }
            catch (Exception)
            {
            }

            return result;
        }
    }
}
