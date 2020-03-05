using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;

namespace OpenMTS.Repositories.Mocking
{
    public class MockDataProvider
    {
        private PasswordHashingService PasswordHashingService { get; }

        public Dictionary<string, User> Users { get; }
        public Dictionary<Guid, StorageSite> StorageSites { get; }
        public Dictionary<string, MaterialType> MaterialTypes { get; }
        public Dictionary<int, Material> Materials { get; }

        public MockDataProvider(PasswordHashingService passwordHashingService)
        {
            PasswordHashingService = passwordHashingService;

            Users = new Dictionary<string, User>();
            StorageSites = new Dictionary<Guid, StorageSite>();
            MaterialTypes = new Dictionary<string, MaterialType>();
            Materials = new Dictionary<int, Material>();

            GenerateUsers();
            GenerateLocations();
            GenerateMaterialTypes();
            GenerateMaterials();
        }

        #region Users

        private void GenerateUsers()
        {
            GenerateUser("alex", "Alexandre Charoy", Role.Administrator);
            GenerateUser("patrick", "Patrick Sapel", Role.ScientificAssistant);
            GenerateUser("max", "Max Mustermann");
        }

        private void GenerateUser(string id, string name, Role role = Role.User)
        {
            (string hash, byte[] salt) = PasswordHashingService.HashAndSaltPassword("test");
            User user = new User()
            {
                Id = id,
                Password = hash,
                Salt = salt,
                Name = name,
                Role = role
            };
            Users.Add(user.Id, user);
        }

        #endregion 

        #region Locations

        private void GenerateLocations()
        {
            GenerateLocation("Pontstr. Keller", new string[] { "Regal links", "Palette rechts", "Palette hinten" });
            GenerateLocation("Pontstr. Empore Maschinenhalle", new string[] { "Abstellplatz links", "Abstellplatz rechts", "Palette hinten" });
        }

        private void GenerateLocation(string name, string[] areas)
        {
            Guid id = Guid.NewGuid();
            StorageSite site = new StorageSite()
            {
                Id = id,
                Name = name,
                Areas = new List<StorageArea>()
            };
            foreach (string area in areas)
            {
                site.Areas.Add(new StorageArea(area));
            }
            StorageSites.Add(id, site);
        }

        #endregion

        #region MaterialTypes

        private void GenerateMaterialTypes()
        {
            GenerateMaterialType("EP", "Epoxy");
            GenerateMaterialType("IR", "Polyisoprene");
            GenerateMaterialType("MN", "Phenol formaldehyde resin");
            GenerateMaterialType("NR", "Natural rubber");
            GenerateMaterialType("PA", "Polyamid");
            GenerateMaterialType("PC", "Polycarbonate");
            GenerateMaterialType("PE", "Polyethylen");
            GenerateMaterialType("PP", "Polypropylene");
            GenerateMaterialType("PUR", "Polyurethane");
            GenerateMaterialType("PVP", "Polyvinylpyrrolidone");
            GenerateMaterialType("S", "Spice");
            GenerateMaterialType("UP", "Unsaturated Polyester");
        }

        private void GenerateMaterialType(string id, string name)
        {
            MaterialTypes.Add(id, new MaterialType()
            {
                Id = id,
                Name = name
            });
        }

        #endregion

        #region Materials

        private void GenerateMaterials()
        {
            GenerateMaterial(1, "PP 505 Standard", "Acme Coproration", "0815", "PP");
            GenerateMaterial(2, "PP 505 ENHANCED", "Acme Coproration - New Tech Branch", "Over-9000", "PP");
            GenerateMaterial(3, "PUR3 G3", "Gabba", "purg3", "PUR");
            GenerateMaterial(4, "EPGlide7", "Awesome Surfboard Glassing Company", "ep-g-7", "EP");
            GenerateMaterial(5, "Spice Melange", "CHOAM", "0001", "S");
        }

        private void GenerateMaterial(int id, string name, string manufacturer, string manufacturerId, string type)
        {
            Material material = new Material()
            {
                Id = id,
                Name = name,
                Manufacturer = manufacturer,
                ManufacturerSpecificId = manufacturerId,
                Type = MaterialTypes.GetValueOrDefault(type),
                CustomProps = new List<CustomMaterialPropValue>()
            };
            Materials.Add(material.Id, material);
        }
        #endregion
    }
}
