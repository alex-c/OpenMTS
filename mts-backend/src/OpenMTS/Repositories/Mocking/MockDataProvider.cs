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
        public Dictionary<string, Plastic> Plastics { get; }
        public Dictionary<int, Material> Materials { get; }
        public Dictionary<Guid, CustomMaterialProp> CustomMaterialProps { get; }
        public Dictionary<Guid, ApiKey> ApiKeys { get; }

        public MockDataProvider(PasswordHashingService passwordHashingService)
        {
            PasswordHashingService = passwordHashingService;

            Users = new Dictionary<string, User>();
            StorageSites = new Dictionary<Guid, StorageSite>();
            Plastics = new Dictionary<string, Plastic>();
            Materials = new Dictionary<int, Material>();
            CustomMaterialProps = new Dictionary<Guid, CustomMaterialProp>();
            ApiKeys = new Dictionary<Guid, ApiKey>();

            GenerateUsers();
            GenerateLocations();
            GeneratePlastics();
            GenerateMaterials();
            GenerateCustomMaterialProps();
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

        #region Plastics
        private void GeneratePlastics()
        {
            GeneratePlastic("EP", "Epoxy");
            GeneratePlastic("IR", "Polyisoprene");
            GeneratePlastic("MN", "Phenol formaldehyde resin");
            GeneratePlastic("NR", "Natural rubber");
            GeneratePlastic("PA", "Polyamid");
            GeneratePlastic("PC", "Polycarbonate");
            GeneratePlastic("PE", "Polyethylen");
            GeneratePlastic("PP", "Polypropylene");
            GeneratePlastic("PUR", "Polyurethane");
            GeneratePlastic("PVP", "Polyvinylpyrrolidone");
            GeneratePlastic("S", "Spice");
            GeneratePlastic("UP", "Unsaturated Polyester");
        }

        private void GeneratePlastic(string id, string name)
        {
            Plastics.Add(id, new Plastic()
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
                Type = Plastics.GetValueOrDefault(type),
                CustomProps = new List<CustomMaterialPropValue>()
            };
            Materials.Add(material.Id, material);
        }
        #endregion

        #region Custom material props

        void GenerateCustomMaterialProps()
        {
            GenerateCustomMaterialProp(new Guid("03003820-fcf4-4a50-bba8-e55488ffac23"), "Datenblatt", PropType.File);
            GenerateCustomMaterialProp(new Guid("c478334c-48ce-4b9a-8e7a-4e01474f3fba"), "Notizen", PropType.Text);
        }

        void GenerateCustomMaterialProp(Guid id, string name, PropType type)
        {
            CustomMaterialProps.Add(id, new CustomMaterialProp()
            {
                Id = id,
                Name = name,
                Type = type
            });
        }

        #endregion
    }
}
