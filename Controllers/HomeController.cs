using Microsoft.AspNetCore.Mvc;
using TPLOCAL1.Models;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace TPLOCAL1.Controllers
{
    public class HomeController : Controller
    {
        // Méthode d'index, appelée par le router
        public ActionResult Index(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return View();
            else
            {
                switch (id)
                {
                    case "OpinionList":
                        var opinions = ReadOpinionsFromXml();
                        return View("OpinionList", opinions);
                    case "Form":
                        var formModel = new FormModel(); // Créez un modèle de formulaire vide
                        return View("Form", formModel);
                    default:
                        return View();
                }
            }
        }
        public IActionResult Form()
        {
            // Assurez-vous que FormModel est correctement initialisé
            var formModel = new FormModel
            {
                Form = "Sélectionner une formation" // Initialisez la valeur par défaut
            };

            return View(formModel);
        }
        private void SaveOpinionToXml(FormModel model)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            string filePath = Path.Combine(folderPath, "opinions.xml");

            // Vérifiez si le dossier existe, sinon créez-le
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            // Si le fichier n'existe pas, créez-le avec une structure de base
            if (!System.IO.File.Exists(filePath))
            {
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    writer.WriteLine("<Opinions></Opinions>");
                }
            }

            // Charger le fichier XML existant
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Créer un nouvel avis
            var newOpinion = xmlDoc.CreateElement("Opinion");
            newOpinion.AppendChild(CreateElement(xmlDoc, "LastName", model.Nom));
            newOpinion.AppendChild(CreateElement(xmlDoc, "FirstName", model.Prenom));
            //newOpinion.AppendChild(CreateElement(xmlDoc, "OpinionGiven", model.Dotnet == "Oui" || model.Cobol == "Oui" ? "O" : "N"));

            string opinionGiven = (model.Cobol != "Non spécifié" && model.Dotnet != "Non spécifié") ? "O" : "N";
            newOpinion.AppendChild(CreateElement(xmlDoc, "OpinionGiven", opinionGiven));

            // Ajouter le nouvel avis au fichier
            xmlDoc.DocumentElement.AppendChild(newOpinion);

            // Sauvegarder les modifications
            xmlDoc.Save(filePath);
        }

        // Méthode utilitaire pour créer des éléments XML
        private XmlElement CreateElement(XmlDocument doc, string name, string value)
        {
            var element = doc.CreateElement(name);
            element.InnerText = value;
            return element;
        }

        // Méthode pour lire les avis depuis un fichier XML
        private List<Opinion> ReadOpinionsFromXml()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "opinions.xml");
            if (!System.IO.File.Exists(filePath))
            {
                return new List<Opinion>();  // Retourne une liste vide si le fichier n'existe pas
            }

            var serializer = new XmlSerializer(typeof(List<Opinion>), new XmlRootAttribute("Opinions"));
            using (var reader = new StreamReader(filePath))
            {
                return (List<Opinion>)serializer.Deserialize(reader);
            }
        }

        // Méthode pour envoyer les données du formulaire à la page de validation
        [HttpPost]
        public ActionResult ValidationFormulaire(FormModel model)
        {
            ModelState.Remove("Cobol");
            ModelState.Remove("Dotnet");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Cobol))
                {
                    model.Cobol = "Non spécifié";
                }

                if (string.IsNullOrWhiteSpace(model.Dotnet))
                {
                    model.Dotnet = "Non spécifié";
                }
                // Enregistrer les données dans le fichier XML
                SaveOpinionToXml(model);

                // Créez un FormationModel et envoyez-le à la vue ValidationForm
                var formationModel = new FormationModel { InfoPage = model };
                return View("ValidationForm", formationModel);
                //return RedirectToAction("ValidationForm", new { model = formationModel });
            }
            // Si le modèle est invalide, retourner à la vue Form avec des erreurs
            return View("Form", model);
        }

        

       
    }
}

