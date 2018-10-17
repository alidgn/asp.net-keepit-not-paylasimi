using KeepIT.Common.Helpers;
using KeepIT.DataAccessLayer.EntityFramework;
using KeepIT.Entities;
using KeepIT.Entities.Messages;
using KeepIT.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepIT.BusinessLayer
{
    public class KeepITUserManager
    {
        private Repository<KeepITUser> repo_user = new Repository<KeepITUser>();

        public BusinessLayerResult<KeepITUser> RegisterUser(RegisterViewModel data)
        {
            // User ve Eposta kontrolü
            // Kayıt işlemi...
            // Aktivasyon epostası gönderimi
            KeepITUser user = repo_user.Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();

            if (user != null)
            {
                if(user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCodes.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if(user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCodes.EmailAlreadyExists,"E-posta adresi kayıtlı.");
                }
            }
            else
            {
                int dbresult = repo_user.Insert(new KeepITUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    Password = data.Password,
                    ProfilePhotoPath = null,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive =false,
                    IsAdmin = false
                });
                if(dbresult > 0)
                {
                    res.Result = repo_user.Find(x => x.Email == data.Email && x.Username == data.Username);

                    // TODO: aktivasyon maili atılacak..
                    //layerResult.Result.ActivateGuid;

                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string activateUri = $"{siteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username}, <br /><br />Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>.";
                    string to = res.Result.Email;
                    string subject = "KeepIT Hesap Aktifleştirme";

                    MailHelper.SendMail(body,to,subject);
                }
            }
            return res;
        }

        public BusinessLayerResult<KeepITUser> GetUserById(int id)
        {
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();
            res.Result = repo_user.Find(x => x.Id == id);
            if(res.Result == null)
            {
                res.AddError(ErrorMessageCodes.UserNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public BusinessLayerResult<KeepITUser> LoginUser(LoginViewModel data)
        {
            // Giriş kontrolü
            // Hesap aktive edilmiş mi
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();
            res.Result = repo_user.Find(x => (x.Username == data.Username && x.Password == data.Password) || (x.Email == data.Username && x.Password == data.Password));


            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCodes.UserIsNotActive, "Kullanıcı aktif değil.");
                    res.AddError(ErrorMessageCodes.CheckYourEmail, "Lütfen mailinizi kontrol ediniz.");

                }
            }
            else
            {
                res.AddError(ErrorMessageCodes.UsernameOrPassWrong, "Kullanıcı adı veya şifre hatalı");
            }

            return res;
        }

        public BusinessLayerResult<KeepITUser> UpdateProfile(KeepITUser data)
        {
            KeepITUser db_user = repo_user.Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();

            if (db_user != null && db_user.Id != data.Id)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCodes.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCodes.EmailAlreadyExists, "E-posta adresi kayıtlı.");
                }
                return res;
            }

            res.Result = repo_user.Find(x => x.Id == data.Id);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;

            if (string.IsNullOrEmpty(data.ProfilePhotoPath) == false)
            {
                res.Result.ProfilePhotoPath = data.ProfilePhotoPath;
            }

            if (repo_user.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCodes.ProfileCouldNotUpdated, "Profil güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<KeepITUser> RemoveUserById(int id)
        {
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();
            KeepITUser user = repo_user.Find(x => x.Id == id);

            if (user != null)
            {
                if (repo_user.Delete(user) == 0)
                {
                    res.AddError(ErrorMessageCodes.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCodes.UserCouldNotFind, "Kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<KeepITUser> ActivateUser(Guid activateId)
        {
            BusinessLayerResult<KeepITUser> res = new BusinessLayerResult<KeepITUser>();

            res.Result = repo_user.Find(x => x.ActivateGuid == activateId);

            if(res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCodes.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }
                res.Result.IsActive = true;
                repo_user.Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCodes.InvalidActivateId, "Aktifleştirilecek kullanıcı bulunamadı.");
            }
            return res;
        }
    }
}
