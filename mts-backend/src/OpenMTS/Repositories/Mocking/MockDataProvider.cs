using OpenMTS.Models;
using OpenMTS.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMTS.Repositories.Mocking
{
    public class MockDataProvider
    {
        private PasswordHashingService PasswordHashingService { get; }

        private Random Random { get; }

        public Dictionary<string, Plastic> Plastics { get; }
        public Dictionary<int, Material> Materials { get; }
        public Dictionary<Guid, StorageSite> StorageSites { get; }
        public Dictionary<Guid, List<Transaction>> BatchTransactions { get; }
        public Dictionary<Guid, MaterialBatch> MaterialBatches { get; }
        public Dictionary<Guid, CustomMaterialProp> CustomMaterialProps { get; }
        public Dictionary<Guid, CustomBatchProp> CustomBatchProps { get; }
        public Dictionary<string, User> Users { get; }
        public Dictionary<Guid, ApiKey> ApiKeys { get; }

        public MockDataProvider(PasswordHashingService passwordHashingService)
        {
            PasswordHashingService = passwordHashingService;
            Random = new Random();

            Plastics = new Dictionary<string, Plastic>();
            Materials = new Dictionary<int, Material>();
            StorageSites = new Dictionary<Guid, StorageSite>();
            BatchTransactions = new Dictionary<Guid, List<Transaction>>();
            MaterialBatches = new Dictionary<Guid, MaterialBatch>();
            CustomMaterialProps = new Dictionary<Guid, CustomMaterialProp>();
            CustomBatchProps = new Dictionary<Guid, CustomBatchProp>();
            Users = new Dictionary<string, User>();
            ApiKeys = new Dictionary<Guid, ApiKey>();

            GeneratePlastics();
            GenerateMaterials();
            GenerateLocations();
            GenerateCustomBatchProps();
            GenerateBatches();
            GenerateCustomMaterialProps();
            GenerateUsers();
        }

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

        #region Locations

        private void GenerateLocations()
        {
            GenerateLocation("2f02b4f2-fc8c-455f-b05f-869d6ab9408c", "Pontstr. Keller", new string[] { "Regal links open", "Regal links unten", "Palette rechts", "Palette hinten" });
            GenerateLocation("caf0520c-74b4-43ba-96b8-06635c25e919", "Pontstr. Empore Maschinenhalle", new string[] { "Abstellplatz links", "Abstellplatz rechts" });
            GenerateLocation("825ff2e8-a057-4434-8279-60a9f4ddbbdf", "Melaten Raum 007", new string[] { "Regal 1", "Regal 2", "Regal 3", "Regal 4" });
        }

        private void GenerateLocation(string guid, string name, string[] areas)
        {
            Guid id = new Guid(guid);
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

        #region Custom batch props

        void GenerateCustomBatchProps()
        {
            GenerateCustomBatchProp(new Guid("512eb3cc-6691-4377-bdcc-3fb7280513f8"), "Verantwortlicher");
        }

        void GenerateCustomBatchProp(Guid id, string name)
        {
            CustomBatchProps.Add(id, new CustomBatchProp()
            {
                Id = id,
                Name = name
            });
        }

        #endregion

        #region Material batches

        private void GenerateBatches()
        {
            StorageSite pontStrKeller = StorageSites[new Guid("2f02b4f2-fc8c-455f-b05f-869d6ab9408c")];
            StorageSite melatenRaum07 = StorageSites[new Guid("825ff2e8-a057-4434-8279-60a9f4ddbbdf")];
            StorageLocation loc1 = new StorageLocation(pontStrKeller, pontStrKeller.Areas.First());
            StorageLocation loc2 = new StorageLocation(melatenRaum07, melatenRaum07.Areas.First());
            string pat = "Patrick Sapel";
            string ann = "Anna A. Annamann";
            GenerateBatch(1, loc1, 34, pat);
            GenerateBatch(1, loc1, 35, pat);
            GenerateBatch(1, loc2, 36, ann);
            GenerateBatch(2, loc1, 42, pat);
            GenerateBatch(3, loc2, 1, "Obelix");
            GenerateBatch(4, loc2, 203, "Obelix");
            GenerateBatch(5, loc1, 22, pat);
            GenerateBatch(5, loc2, 9000, pat);
        }

        private void GenerateBatch(int materialId, StorageLocation storageLocation, long batchNumber, string verantwortlicher)
        {
            MaterialBatch batch = new MaterialBatch()
            {
                Id = Guid.NewGuid(),
                Material = Materials.GetValueOrDefault(materialId),
                StorageLocation = storageLocation,
                BatchNumber = batchNumber,
                ExpirationDate = DateTime.UtcNow.AddDays(Random.Next(3, 50)),
                Quantity = Random.Next(35, 500),
                IsLocked = false,
                IsArchived = false,
                CustomProps = new Dictionary<Guid, string>()
                {
                    { new Guid("512eb3cc-6691-4377-bdcc-3fb7280513f8"), verantwortlicher }
                }
            };
            MaterialBatches.Add(batch.Id, batch);
            BatchTransactions.Add(batch.Id, new List<Transaction>()
            {
                new Transaction()
                {
                    Id = Guid.NewGuid(),
                    MaterialBatchId = batch.Id,
                    UserId = "alex",
                    Quantity = batch.Quantity,
                    Timestamp = DateTime.UtcNow
                }
            });
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
    }
}
