using System;
using System.Net.Mime;
using Project.ConstructionTracking.Web.Commons;
using Project.ConstructionTracking.Web.Data;
using Project.ConstructionTracking.Web.Models;
using Project.ConstructionTracking.Web.Models.MUserModel;
using static Project.ConstructionTracking.Web.Models.FormGroupModel;

namespace Project.ConstructionTracking.Web.Repositories
{
	public interface IMasterUserRepo
	{
		dynamic GetUserList(DTParamModel param, MasterUserModel criteria);
        dynamic GetBU();
        dynamic GetRole();
        dynamic GetPosition();

        dynamic CreateUser(CreateUserModel model);

        dynamic EditUser(EditUserModel model);
        dynamic DetailUser(Guid userId);

        Resources UploadSignResource(string model, string appPath, Guid userId, Guid requestUserID);

        bool DeleteUser(Guid userId, Guid requestUserID);
    }

	public class MasterUserRepo : IMasterUserRepo
	{
		private readonly ContructionTrackingDbContext _context;
		public MasterUserRepo(ContructionTrackingDbContext context)
		{
			_context = context;
		}

		public dynamic GetUserList(DTParamModel param, MasterUserModel criteria)
		{
            //throw new NotImplementedException();
            var totalRecord = 0;
            bool asc = param.sortDirection.ToUpper().Contains("ASC");

            //variable = (condition) ? expressionTrue :  expressionFalse;
            criteria.strSearch = (criteria.strSearch == null) ? string.Empty : criteria.strSearch.ToString() ?? string.Empty;

            var query = from user in _context.tm_User
                        join role in _context.tm_Role on user.RoleID equals role.ID
                        join pst in _context.tm_Position on user.PositionID equals pst.ID
                        where user.FlagActive == true
                        select new
                        {
                            user.ID,
                            user.FirstName,
                            user.LastName,
                            user.Email,
                            user.Mobile,
                            user.PositionID,
                            PositionName = pst.Name,
                            user.RoleID,
                            role.Name,
                            user.CreateDate
                        };

            if (!string.IsNullOrEmpty(criteria.strSearch))
            {
                query = query.Where(o =>
                            o.FirstName.Contains(criteria.strSearch) ||
                            o.LastName.Contains(criteria.strSearch) ||
                            o.Email.Contains(criteria.strSearch) ||
                            o.Mobile.Contains(criteria.strSearch) ||
                            (o.PositionID.HasValue && o.PositionID.Value.ToString().Contains(criteria.strSearch)) ||
                            (o.RoleID.HasValue && o.RoleID.Value.ToString().Contains(criteria.strSearch))
                        );
            }

            var result = query.Page(param.start, param.length, i => i.CreateDate, param.SortColumnName, asc, out totalRecord);
            param.TotalRowCount = totalRecord;
            return result.AsEnumerable().Select(e => new
            {
                UserID = e.ID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Mobile = e.Mobile,
                PositionID = e.PositionID,
                PositionName = e.PositionName,
                RoleID = e.RoleID,
                RoleName = e.Name,
            }).ToList();
            
 		}

        public dynamic CreateUser(CreateUserModel model)
        {
            tm_User create = new tm_User();
            tm_User? verifyEmail = _context.tm_User.Where(o => o.Email == model.Email && o.FlagActive == true).FirstOrDefault();
            if (verifyEmail != null) throw new Exception("อีเมลล์นี้ถูกใช้งานแล้ว");
            else
            {
                create.ID = Guid.NewGuid();
                create.BUID = model.BUID;
                //create.DepartmentID = ;
                create.RoleID = model.RoleID;
                create.Username = model.Email;

                // change to gen md5 
                create.Password = HashExtension.EncryptMD5(model.Password, model.PasswordKey);

                // check Position by check master position
                tm_Position? position = _context.tm_Position
                                    .Where(o => o.Name == model.JobPosition && o.FlagActive == true)
                                    .FirstOrDefault();

                if (position == null)
                {
                    tm_Position newData = new tm_Position();
                    newData.Name = model.JobPosition;
                    newData.FlagActive = true;
                    newData.CreateDate = DateTime.Now;
                    newData.UpdateDate = DateTime.Now;
                    newData.CreateBy = model.RequestUserID;
                    newData.UpdateBy = model.RequestUserID;

                    _context.tm_Position.Add(newData);
                    _context.SaveChanges();

                    create.PositionID = newData.ID;
                }
                else
                {
                    create.PositionID = position.ID;
                }

                create.FirstName = model.FirstName;
                create.LastName = model.LastName;
                create.Email = model.Email;
                create.Mobile = model.MobileNo;
                create.FlagActive = true;
                create.CreateDate = DateTime.Now;
                create.UpdateDate = DateTime.Now;
                create.CreateBy = model.RequestUserID;
                create.UpdateBy = model.RequestUserID;

                _context.tm_User.Add(create);
                _context.SaveChanges();
            }
            return create;
        }

        public dynamic EditUser(EditUserModel model)
        {
            tm_User? edit = _context.tm_User.Where(o => o.ID == model.UserID && o.FlagActive == true).FirstOrDefault();
            if (edit == null) throw new Exception("ไม่พบข้อมูลยูนิต");

            edit.BUID = model.BUID;
            //create.DepartmentID = ;
            edit.RoleID = model.RoleID;
            edit.Username = model.Email;

            if (model.Password != null && model.ConfirmPassword != null)
            {
                // change to gen md5 
                edit.Password = HashExtension.EncryptMD5(model.Password, model.KeyPassword);
            }

            // check Position by check master position
            tm_Position? position = _context.tm_Position
                                .Where(o => o.Name == model.JobPosition && o.FlagActive == true)
                                .FirstOrDefault();
            if (position == null)
            {
                tm_Position newData = new tm_Position();
                newData.Name = model.JobPosition;
                newData.FlagActive = true;
                newData.CreateDate = DateTime.Now;
                newData.UpdateDate = DateTime.Now;
                newData.CreateBy = model.RequestUserID;
                newData.UpdateBy = model.RequestUserID;

                _context.tm_Position.Add(newData);
                _context.SaveChanges();

                edit.PositionID = newData.ID;
            }
            else
            {
                edit.PositionID = position.ID;
            }

            edit.FirstName = model.FirstName;
            edit.LastName = model.LastName;
            edit.Email = model.Email;
            edit.Mobile = model.MobileNo;
            edit.UpdateDate = DateTime.Now;
            edit.UpdateBy = model.RequestUserID;

            _context.tm_User.Update(edit);
            _context.SaveChanges();

            return edit;
        }

        public dynamic GetBU()
        {
            var query = (from bu in _context.tm_BU
                         where bu.FlagActive == true
                         select new
                         {
                             bu.ID,
                             bu.Name,
                         }).ToList();

            return query;
        }

        public dynamic GetRole()
        {
            var query = (from role in _context.tm_Role
                         where role.FlagActive == true
                         select new
                         {
                             role.ID,
                             role.Name
                         }).ToList();

            return query;
        }

        public dynamic GetPosition()
        {
            var query = (from pst in _context.tm_Position
                         where pst.FlagActive == true
                         select new
                         {
                             pst.ID,
                             pst.Name
                         }).ToList();

            return query;
        }

        public dynamic DetailUser(Guid userId)
        {
            var query = (from user in _context.tm_User
                         join pst in _context.tm_Position on user.PositionID equals pst.ID
                         join ur in _context.tr_UserResource on user.ID equals ur.UserID into groupUr
                         from ur in groupUr.DefaultIfEmpty()
                         join r in _context.tm_Resource on ur.ResourceID equals r.ID into groupR
                         from r in groupR.DefaultIfEmpty()
                         where user.ID == userId
                               && user.FlagActive == true
                               && (r.FlagActive == true || r.ID == null)
                         select new
                         {
                             user.ID,
                             user.FirstName,
                             user.LastName,
                             user.Email,
                             user.Mobile,
                             user.BUID,
                             user.RoleID,
                             user.PositionID,
                             pst.Name,
                             r.FilePath
                         }).FirstOrDefault();

            return query;
        }

        public Resources UploadSignResource(string model, string appPath, Guid userId, Guid RequestUserID)
        {
            Resources resource = new Resources();
            Guid guidId = Guid.NewGuid();
            string filePath = "";

            if (model != null)
            {
                string fileName = guidId + ".jpg";

                var folder = DateTime.Now.ToString("yyyyMM");
                var dirPath = $"Upload/document/{folder}/sign/";
                filePath = dirPath + fileName;

                resource.PhysicalPathServer = appPath;
                resource.ResourceStorageBase64 = model;
                resource.ResourceStoragePath = filePath;
                resource.Directory = Path.Combine(appPath, dirPath);
                ConvertByteToImage(resource);

                CreateUploadSign(userId, fileName, filePath, RequestUserID);
            }

            return resource;
        }
        private void ConvertByteToImage(Resources item)
        {
            // Convert the Base64 UUEncoded input into binary output. 
            byte[] binaryData;
            try
            {
                binaryData =
                   System.Convert.FromBase64String(item.ResourceStorageBase64);
            }
            catch (System.ArgumentNullException)
            {
                System.Console.WriteLine("Base 64 string is null.");
                return;
            }
            catch (System.FormatException ex)
            {
                throw ex;
            }

            // Write out the decoded data.
            System.IO.FileStream outFile;
            try
            {
                if (!Directory.Exists(item.Directory))
                {
                    Directory.CreateDirectory(item.Directory);
                }
                //var pathFile = string.Format("{0}{1}", item.PhysicalPathServer, item.ResourceStoragePath);
                outFile = new System.IO.FileStream(item.ResourceStoragePath,
                                           System.IO.FileMode.Create,
                                           System.IO.FileAccess.Write);
                outFile.Write(binaryData, 0, binaryData.Length);
                outFile.Close();
            }
            catch (System.Exception exp)
            {
                // Error creating stream or writing to it.
                throw exp;
            }
        }

        public bool CreateUploadSign(Guid userId, string fileName, string filePath, Guid requestUserID)
        {
            tr_UserResource? userResource = _context.tr_UserResource
                                        .Where(o => o.UserID == userId && o.FlagActive == true).FirstOrDefault();

            if (userResource == null)
            {
                tm_Resource createResource = new tm_Resource();
                createResource.ID = Guid.NewGuid();
                createResource.FileName = fileName;
                createResource.FilePath = filePath;
                createResource.MimeType = "image/jpg";
                createResource.FlagActive = true;
                createResource.CreateDate = DateTime.Now;
                createResource.UpdateDate = DateTime.Now;
                createResource.CreateBy = requestUserID;
                createResource.UpdateBy = requestUserID;

                _context.tm_Resource.Add(createResource);
                _context.SaveChanges();

                tr_UserResource mappingUser = new tr_UserResource();
                mappingUser.ResourceID = createResource.ID;
                mappingUser.UserID = userId;
                mappingUser.FlagActive = true;
                mappingUser.CreateDate = DateTime.Now;
                mappingUser.UpdateDate = DateTime.Now;
                mappingUser.CreateBy = requestUserID;
                mappingUser.UpdateBy = requestUserID;

                _context.tr_UserResource.Add(mappingUser);
                _context.SaveChanges();
            }
            else
            {
                //update mapping user resource to false
                userResource.FlagActive = false;
                userResource.UpdateDate = DateTime.Now;
                userResource.UpdateBy = requestUserID;

                _context.tr_UserResource.Update(userResource);

                // update image to false
                tm_Resource? resource = _context.tm_Resource
                                        .Where(o => o.ID == userResource.ResourceID
                                        && o.FlagActive == true).FirstOrDefault();
                resource.FlagActive = false;
                resource.FlagActive = false;
                resource.UpdateDate = DateTime.Now;
                userResource.UpdateBy = requestUserID;

                _context.tm_Resource.Update(resource);

                //create new image 
                tm_Resource createResource = new tm_Resource();
                createResource.ID = Guid.NewGuid();
                createResource.FileName = fileName;
                createResource.FilePath = filePath;
                createResource.MimeType = "image/jpg";
                createResource.FlagActive = true;
                createResource.CreateDate = DateTime.Now;
                createResource.UpdateDate = DateTime.Now;
                createResource.CreateBy = requestUserID;
                createResource.UpdateBy = requestUserID;

                _context.tm_Resource.Add(createResource);
                _context.SaveChanges();

                //create new mapping user resource 
                tr_UserResource mappingUser = new tr_UserResource();
                mappingUser.ResourceID = createResource.ID;
                mappingUser.UserID = userId;
                mappingUser.FlagActive = true;
                mappingUser.CreateDate = DateTime.Now;
                mappingUser.UpdateDate = DateTime.Now;
                mappingUser.CreateBy = requestUserID;
                mappingUser.UpdateBy = requestUserID;

                _context.tr_UserResource.Add(mappingUser);
                _context.SaveChanges();
            }
            return true;
        }

        public bool DeleteUser(Guid userId, Guid requestUserID)
        {
            tm_User? delete = _context.tm_User
                            .Where(o => o.ID == userId && o.FlagActive == true).FirstOrDefault();

            if (delete == null) throw new Exception("ไม่พบข้อมูลผู้ใช้งาน");

            delete.FlagActive = false;
            delete.UpdateDate = DateTime.Now;
            delete.UpdateBy = requestUserID;

            _context.tm_User.Update(delete);
            _context.SaveChanges();

            return true;
        }
    }
}

