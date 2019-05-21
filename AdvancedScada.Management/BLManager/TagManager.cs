﻿using System;
using System.Collections.Generic;
using System.Xml;
using AdvancedScada.DriverBase.Devices;

namespace AdvancedScada.Management.BLManager
{

    public class TagManagerXML
    {
        public const string TAG = "Tag";
        public const string CHANNEL_ID = "ChannelId";
        public const string DEVICE_ID = "DeviceId";
        public const string DATABLOCK_ID = "DataBlockId";
        public const string TAG_ID = "TagId";
        public const string TAG_NAME = "TagName";
        public const string ADDRESS = "Address";
        public const string DATA_TYPE = "DataType";

        private static readonly object mutex = new object();
        private static TagManagerXML _instance;

        public static TagManagerXML GetTagManager()
        {
            lock (mutex)
            {
                if (_instance == null) _instance = new TagManagerXML();
            }

            return _instance;
        }
        /// <summary>
        ///     Thêm mới gói dữ liệu.
        /// </summary>
        /// <param name="db">gói dữ liệu</param>
        /// <param name="db">gói dữ liệu</param>
        public void Add(DataBlock db, Tag tg)
        {
            try
            {
                if (tg == null) throw new NullReferenceException("The Tag is null reference exception");
                IsExisted(db, tg);
                db.Tags.Add(tg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Cập nhật gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tg">gói dữ liệu</param>
        public void Update(DataBlock db, Tag tg)
        {
            try
            {
                if (tg == null) throw new NullReferenceException("The Tag is null reference exception");
                IsExisted(db, tg);
                foreach (var item in db.Tags)
                    if (item.TagId == tg.TagId)
                    {
                        item.TagId = tg.TagId;
                        item.TagName = tg.TagName;
                        item.Address = tg.Address;
                        item.DataType = tg.DataType;

                        break;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Xóa gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tgId">Mã gói dữ liệu</param>
        public void Delete(DataBlock db, int tgId)
        {
            try
            {
                var result = GetByTagId(db, tgId);
                if (result == null) throw new KeyNotFoundException("Tag Id is not found exception");
                db.Tags.Remove(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Xóa gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="dbName">Tên gói dữ liệu</param>
        public void Delete(DataBlock db, string dbName)
        {
            try
            {
                var result = GetByTagName(db, dbName);
                if (result == null) throw new KeyNotFoundException("Tag name is not found exception");
                db.Tags.Remove(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Xóa gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tg">gói dữ liệu</param>
        public void Delete(DataBlock db, Tag tg)
        {
            try
            {
                if (tg == null) throw new NullReferenceException("The Tag is null reference exception");
                foreach (var item in db.Tags)
                    if (item.TagId == tg.TagId)
                    {
                        db.Tags.Remove(item);
                        break;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///     Phương thức kiểm tra gói dữ liệu đã tồn tại chưa?
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tg">gói dữ liệu</param>
        /// <returns>gói dữ liệu</returns>
        public Tag IsExisted(DataBlock db, Tag tg)
        {
            Tag result = null;
            try
            {
                foreach (var item in db.Tags)
                {
                    if (item.TagId != tg.TagId && item.TagName.Equals(tg.TagName))
                        throw new InvalidOperationException($"Tag name: '{tg.TagName}' is existed");
                    if (item.TagId != tg.TagId && item.Address.Equals(tg.Address))
                        throw new InvalidOperationException($"Tag address: '{tg.Address}' is existed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        ///     Tìm kiếm gói dữ liệu theo mã gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tgId">Mã gói dữ liệu</param>
        /// <returns>Gói dữ liệu</returns>
        public Tag GetByTagId(DataBlock db, int tgId)
        {
            Tag result = null;
            try
            {
                foreach (var item in db.Tags)
                    if (item.TagId == tgId)
                    {
                        result = item;
                        break;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        ///     Tìm kiếm gói dữ liệu theo tên gói dữ liệu.
        /// </summary>
        /// <param name="db">Thiết bị</param>
        /// <param name="tgName">Tên gói dữ liệu</param>
        /// <returns>Gói dữ liệu</returns>
        public Tag GetByTagName(DataBlock db, string tgName)
        {
            Tag result = null;
            try
            {
                foreach (var item in db.Tags)
                    if (item.TagName.Equals(tgName))
                    {
                        result = item;
                        break;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }


        /// <summary>
        ///     Tìm kiếm gói dữ liệu theo tên gói dữ liệu.
        /// </summary>
        /// <param name="db">Gói dữ liệu</param>
        /// <param name="tgAddress">địa chỉ tag</param>
        /// <returns>Gói dữ liệu</returns>
        public Tag GetByAddress(DataBlock db, ushort tgAddress)
        {
            Tag result = null;
            try
            {
                foreach (var item in db.Tags)
                    if (item.Address.Equals(tgAddress))
                    {
                        result = item;
                        break;
                    }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        ///     Hàm đọc danh sách các gói dữ liệu.
        /// </summary>
        /// <param name="dbNote">XmlNode</param>
        /// <returns>Danh sách gói dữ liệu</returns>
        public List<Tag> GetTags(XmlNode dbNote)
        {
            var dbList = new List<Tag>();
            try
            {
                foreach (XmlNode item in dbNote)
                {
                    var tg = new Tag();
                    tg.ChannelId = int.Parse(dbNote.Attributes[CHANNEL_ID].Value);
                    tg.DeviceId = int.Parse(dbNote.Attributes[DEVICE_ID].Value);
                    tg.DataBlockId = int.Parse(dbNote.Attributes[DATABLOCK_ID].Value);
                    tg.TagId = int.Parse(item.Attributes[TAG_ID].Value);
                    tg.TagName = item.Attributes[TAG_NAME].Value;
                    tg.Address = item.Attributes[ADDRESS].Value;
                    tg.DataType = $"{item.Attributes[DATA_TYPE].Value}";
                    tg.Desp = item.Attributes[ChannelManager.DESCRIPTION].Value;
                    dbList.Add(tg);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dbList;
        }
    }
}