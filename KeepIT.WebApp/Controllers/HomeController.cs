using KeepIT.BusinessLayer;
using KeepIT.Entities;
using KeepIT.Entities.Messages;
using KeepIT.Entities.ValueObjects;
using KeepIT.WebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace KeepIT.WebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //BusinessLayer.Test test = new BusinessLayer.Test();
            //test.InsertTest();
            //test.UpdateTest();
            //test.DeleteTest();
            //test.CommentTest();


            NoteManager nm = new NoteManager();
            return View(nm.GetNotes().OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CategoryManager cm = new CategoryManager();
            Category cat = cm.GetCategoryById(id.Value); // nullable tiplerde hata verirse .Value yazarak değerini alırız.
            if (cat == null)
            {
                return HttpNotFound();
                //return RedirectToAction("Index", "Home");
            }

            return View("Index", cat.Notes.OrderByDescending(x => x.ModifiedOn).ToList());
        }
        public ActionResult MostLiked()
        {
            //BusinessLayer.Test test = new BusinessLayer.Test();
            //test.InsertTest();
            //test.UpdateTest();
            //test.DeleteTest();
            //test.CommentTest();


            NoteManager nm = new NoteManager();
            return View("Index", nm.GetNotes().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult ShowProfile()
        {
            KeepITUser currentUser = Session["login"] as KeepITUser;
            KeepITUserManager kum = new KeepITUserManager();
            BusinessLayerResult<KeepITUser> res = kum.GetUserById(currentUser.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Bir Hata Oluştu.",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        public ActionResult EditProfile()
        {
            KeepITUser currentUser = Session["login"] as KeepITUser;
            KeepITUserManager kum = new KeepITUserManager();
            BusinessLayerResult<KeepITUser> res = kum.GetUserById(currentUser.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Bir Hata Oluştu.",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }
            return View(res.Result);
        }

        [HttpPost]
        public ActionResult EditProfile(KeepITUser model, HttpPostedFileBase ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && (ProfileImage.ContentType == "image/jpeg" || ProfileImage.ContentType == "image/jpg" || ProfileImage.ContentType == "image/png"))
                {
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    model.ProfilePhotoPath = filename;
                }

                KeepITUserManager kum = new KeepITUserManager();
                BusinessLayerResult<KeepITUser> res = kum.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotify = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "/Home/EditProfile",
                        RedirectingTimeout = 5000
                    };
                    return View("Error", errorNotify);
                }

                // Profil güncellendiği için session güncellendi.
                Session["login"] = res.Result;

                return RedirectToAction("ShowProfile");
            }
            return View(model);
        }


        public ActionResult DeleteProfile()
        {
            KeepITUser currentUser = Session["login"] as KeepITUser;
            KeepITUserManager kum = new KeepITUserManager();
            BusinessLayerResult<KeepITUser> res = kum.RemoveUserById(currentUser.Id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi.",
                    RedirectingUrl = "/Home/ShowProfile",
                    RedirectingTimeout = 5000                    
                };

                return View("Error", errorNotifyObj);
            }

            Session.Clear();

            return RedirectToAction("Index");
        }






        public ActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //Giriş Kontrolü
            if (ModelState.IsValid)
            {
                KeepITUserManager kum = new KeepITUserManager();
                BusinessLayerResult<KeepITUser> res = kum.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    if (res.Errors.Find(x => x.Code == ErrorMessageCodes.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "E-Posta Gönder";
                    }
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message)); // Tüm error listesinde foreachle dön her biri için ilgili stringi model errora ekle



                    return View(model);
                }

                Session["login"] = res.Result; // Session'a kullanıcı bilgisi saklama
                return RedirectToAction("Index"); // yönlendirme
            }

            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            // User ve Eposta kontrolü
            // Kayıt işlemi...
            // Aktivasyon epostası gönderimi

            if (ModelState.IsValid)
            {
                KeepITUserManager eum = new KeepITUserManager();
                BusinessLayerResult<KeepITUser> res = eum.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message)); // Tüm error listesinde foreachle dön her biri için ilgili stringi model errora ekle
                    return View(model);
                }
                OkViewModel notify = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login"
                };
                notify.Items.Add("Lütfen e-posta adresinize gönderilen aktivasyon linkine tıklayarak hesabınızı aktif ediniz.");
                return View("Ok", notify);
            }
            return View(model);
        }
        public ActionResult UserActivate(Guid id)
        {
            //Kullanıcı aktivasyonu sağlanacak
            KeepITUserManager kum = new KeepITUserManager();
            BusinessLayerResult<KeepITUser> res = kum.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotify = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors
                };
                return View("Error", errorNotify);
            }

            OkViewModel okNotify = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi.",
                RedirectingUrl = "/Home/Login"
            };
            okNotify.Items.Add("Hesabınız aktifleştirildi. Artık Not paylaşabilirsiniz.");
            return View("Ok", okNotify);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        //public ActionResult TestNotify()
        //{
        //    ErrorViewModel model = new ErrorViewModel()
        //    {

        //        Title = "OK test",
        //        RedirectingTimeout = 3000,
        //        Items = new List<ErrorMessageObj>() {
        //            new ErrorMessageObj() { Message = "Test Başarılı 1" },
        //            new ErrorMessageObj(){ Message ="Test Başarılı 2"} }
        //    };
        //    return View("Error", model);
        //}
    }
}