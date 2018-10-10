﻿using MusicStore.Business.Interfaces;
using System;
using System.Net;
using System.Web.Mvc;

namespace MusicStore.Web.Controllers
{
    public class MusicStoreController: Controller
    {
        private readonly IMusicStoreService _musicStoreService;

        private const int DEFAULT_USER_ID = 1;
        public MusicStoreController(IMusicStoreService musicStoreService)
        {
            _musicStoreService = musicStoreService;
        }

        [Authorize(Roles = "Admin, Registered user")]
        public ActionResult DisplayAvailableMusicForLoggedUser()
        {
            ViewBag.AvailableMusicList = _musicStoreService.DisplayAllAvailableSongs(DEFAULT_USER_ID);
            ViewBag.userId = DEFAULT_USER_ID;
            return View();
        }

        [Authorize(Roles = "Admin, Registered user")]
        [HttpPost]
        public ActionResult BuyMusic(int userId, int songId)
        {
            if (userId < 0 || songId < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "userId or songId is null");
            }
            try
            {
                var resultOfBuy = _musicStoreService.BuySong(songId, userId);
                ViewBag.OperationResult = (resultOfBuy != null) ? "Покупка совершена успешно" : "Покупка не совершена успешно";
            }
            catch (NullReferenceException exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, exception.Message);
            }
            catch (ArgumentException exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, exception.Message);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, exception.Message);
            }

            return View();
        }
    }
}
