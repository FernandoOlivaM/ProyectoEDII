﻿using MongoDB.Driver;
using ProyectoEDII.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoEDII.LogInServices
{
    public class LogInService
    {
        private readonly IMongoCollection<LogInElements> _login;
        public LogInService(ILogInDatabase settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _login = database.GetCollection<LogInElements>(settings.LogInCollectionName);
        }
        public List<LogInElements> GetAll()
        {
            return _login.Find(x => true).ToList();
        }
        public LogInElements Get(string username) =>
           _login.Find<LogInElements>(p => p.UserName == username).FirstOrDefault();
        public LogInElements Insertar(LogInElements user)
        {
            _login.InsertOne(user);
            return user;
        }
        public async void Modificar(string id, LogInElements userelements)
        {
            await _login.ReplaceOneAsync(users => users.Id == id, userelements);
        }
        //tercera pruba
        public async void Eliminar(string username)
        {
            await _login.DeleteOneAsync((x => x.UserName == username));
        }
    }
}
