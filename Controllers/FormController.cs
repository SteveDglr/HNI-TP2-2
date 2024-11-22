using Microsoft.AspNetCore.Mvc;
using TPLOCAL1.Models;

public class FormController : Controller
{
    [HttpPost]
    public ActionResult Create(FormModel model)
    {
        if (ModelState.IsValid)
        {
            // Si le modèle est valide, enregistrer les données, rediriger, etc.
            return RedirectToAction("Success");
        }

        // Si la validation échoue, ajouter des erreurs manuellement
        if (model.Adresse == null || model.Adresse.Length < 5)
        {
            ModelState.AddModelError("Adresse", "L'adresse est trop courte.");
        }

        // Retourner à la vue avec les erreurs de validation
        return View(model);
    }
}
