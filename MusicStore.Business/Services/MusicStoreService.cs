﻿using MusicStore.Business.Interfaces;
using MusicStore.DataAccess.Interfaces;
using MusicStore.Domain;
using System;
using MusicStore.DataAccess;

namespace MusicStore.Business.Services
{
    public class MusicStoreService: IMusicStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Song> _songRepository;
        private readonly IRepository<BoughtSong> _boughtSongRepository;
        private readonly IGenericRepositoryWithPagination<Album> _albumRepository;

        private readonly IMapper<BoughtSong, Domain.DataTransfer.BoughtSong> _mapBoughtSong;

        public MusicStoreService(IUnitOfWork unitOfWork, IMapper<BoughtSong, Domain.DataTransfer.BoughtSong> mapBoughtSong)
        {
            _unitOfWork = unitOfWork;
            _userRepository = unitOfWork.UserAccountRepository;
            _songRepository = unitOfWork.SongRepository;
            _boughtSongRepository = unitOfWork.BoughtSongRepository;
            _albumRepository = unitOfWork.AlbumRepositoryWithPagination;

            _mapBoughtSong = mapBoughtSong;

        }

        public Domain.DataTransfer.BoughtSong BuySong(int songId, int userId)
        {
            if (songId < 0 || userId < 0)
            {
                throw new ArgumentException("userId < 0 or songId < 0 in musicStoreService in BuySong", "userId or songId");
            }

            User user = _userRepository.GetItem(userId);
            Song song = _songRepository.GetItem(songId);

            if (user == null || song == null)
            {
                return null;
            }
            if (user.Money < song.Price)
            {
                throw new Exception($"User has not enough money for buy {song.Name} song");
            }

            BoughtSong boughtSong = new BoughtSong()
            {
                BoughtPrice = song.Price,
                IsVisible = true,
                BoughtDate = DateTime.Now,
                Song = song,
                User = user
            };
            user.Money -= song.Price;
            _boughtSongRepository.Create(boughtSong);
            _userRepository.Update(user);
            var result = _mapBoughtSong.AutoMap(boughtSong);
            return result;
        }
    }
}
