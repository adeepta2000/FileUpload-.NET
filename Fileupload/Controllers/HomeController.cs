using Fileupload.Database;
using Fileupload.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Fileupload.Controllers
{
public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult About()
    {
        ViewBag.Message = "Your application description page.";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

        return View();
    }

    public ActionResult Submit(Student std) 
    {
            FileUploadEntities db = new FileUploadEntities();
            var checkLogin=db.Students.Where(x=>x.StudentId.Equals(std.StudentId)&& x.Password.Equals(std.Password)).FirstOrDefault();

      if(checkLogin!=null) 
       {
            Session["StudentId"]=std.StudentId.ToString();
            TempData["StudentId"] = Session["StudentId"];
            TempData["NewStudentId"] = Session["StudentId"];
                
                return RedirectToAction("Dashboard");
        }

        return View();
            
    }

    public ActionResult Logout() 
    {
        Session.Clear();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public ActionResult Dashboard()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Dashboard(HttpPostedFileBase File, NewUploadFile newUp)
    {
            
        if (File != null && File.ContentLength > 0)
        {
               
            string studentID = (string) TempData["StudentId"];

                
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);

                
            string studentFolder = Path.Combine(Server.MapPath("~/App_Data/StudentFiles"), studentID.ToString());
            if (!Directory.Exists(studentFolder))
            {
                Directory.CreateDirectory(studentFolder);
            }

                
            string filePath = Path.Combine(studentFolder, fileName);
            File.SaveAs(filePath);

     
            using (var db = new FileUploadEntities()) 
            {
                
                var doc = new NewUploadFile
                {
                    Name= newUp.Name,
                    FileName = File.FileName,
                    FilePath = filePath,
                    UploadDate = DateTime.Now,
                    StudentId = studentID
                };
                    Console.WriteLine(doc);

                   db.NewUploadFiles.Add(doc);

                      
                  db.SaveChanges();
                    return RedirectToAction("Dashboard");
                    }
       
               
            }

            
        return View();
    }

        public ActionResult Checkfile()
        {
            FileUploadEntities db = new FileUploadEntities();
            
            string currentUserId = (string)Session["StudentId"];


            var userSpecificData = db.NewUploadFiles.Where(s => s.StudentId == currentUserId).ToList();

            return View(userSpecificData);

           
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var db = new FileUploadEntities();
            var data = db.NewUploadFiles.Find(id);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase File, NewUploadFile newUp)
        {
            if (File != null && File.ContentLength > 0)
            {
                string studentID = (string)TempData["NewStudentId"];

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(File.FileName);


                string studentFolder = Path.Combine(Server.MapPath("~/App_Data/StudentFiles"), studentID.ToString());
                if (!Directory.Exists(studentFolder))
                {
                    Directory.CreateDirectory(studentFolder);
                }


                string filePath = Path.Combine(studentFolder, fileName);
                File.SaveAs(filePath);

                var db = new FileUploadEntities();
                var ex = db.NewUploadFiles.Find(newUp.DocumentId);
                string OldPath = ex.FilePath;
                ex.Name = newUp.Name;
                ex.FileName = File.FileName;
                ex.FilePath= filePath;
                ex.UploadDate = DateTime.Now;
                
                if (db.SaveChanges() > 0)
                {
                    if (System.IO.File.Exists(OldPath))
                    {
                        System.IO.File.Delete(OldPath);
                    }

                  

                    return RedirectToAction("Checkfile");
                }
            }
          
           return View();
        }

        public ActionResult Delete(int id) 
        {
            var db = new FileUploadEntities();
            var exentity = db.NewUploadFiles.Find(id);
            string cufilePath = exentity.FilePath;
            db.NewUploadFiles.Remove(exentity);
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(cufilePath))
                {
                    System.IO.File.Delete(cufilePath);
                }
            
                return RedirectToAction("Checkfile");
            }
            return View();
        }



    }
}