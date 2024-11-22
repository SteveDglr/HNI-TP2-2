using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TPLOCAL1.Models
{
    public class OpinionList
    {
        /// <summary>
        /// Fonction pour récupérer la liste des avis depuis un fichier XML
        /// </summary>
        /// <param name="file">Chemin du fichier XML</param>
        /// <returns>Liste des avis</returns>
        public List<Opinion> GetAvis(string file)
        {
            List<Opinion> opinionList = new List<Opinion>();

            // Vérifier si le fichier existe avant de tenter de le lire
            if (!File.Exists(file))
            {
                throw new FileNotFoundException($"Le fichier spécifié est introuvable : {file}");
            }

            try
            {
                // Utilisation de StreamReader avec le bloc using pour la gestion automatique des ressources
                using (StreamReader streamDoc = new StreamReader(file))
                {
                    string dataXml = streamDoc.ReadToEnd();

                    // Création d'un objet XmlDocument pour charger et manipuler le fichier XML
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(dataXml); // Charger les données XML

                    // Sélection des nœuds correspondant à "root/row"
                    foreach (XmlNode node in xmlDoc.SelectNodes("root/row"))
                    {
                        // Récupération des données dans chaque nœud enfant avec des vérifications nulles
                        string lastName = node["LastName"]?.InnerText ?? "Inconnu";
                        string firstName = node["FirstName"]?.InnerText ?? "Inconnu";
                        string opinionGiven = node["OpinionGiven"]?.InnerText ?? "N";

                        // Création de l'objet Opinion et ajout à la liste
                        Opinion opinion = new Opinion
                        {
                            LastName = lastName,
                            FirstName = firstName,
                            OpinionGiven = opinionGiven
                        };

                        opinionList.Add(opinion);
                    }
                }
            }
            catch (XmlException ex)
            {
                // Gestion des erreurs si le fichier XML est mal formé
                throw new InvalidOperationException("Le fichier XML est mal formé.", ex);
            }
            catch (Exception ex)
            {
                // Autres erreurs générales
                throw new Exception("Une erreur s'est produite lors de la lecture du fichier.", ex);
            }

            return opinionList;
        }
    }

    // Classe représentant un avis
    public class Opinion
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string OpinionGiven { get; set; } // "O" pour donné, "N" pour non donné
    }
}
