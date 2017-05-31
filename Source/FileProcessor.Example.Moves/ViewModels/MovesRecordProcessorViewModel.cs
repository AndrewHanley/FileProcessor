//  ---------------------------------------------
//    Solution:  FileProcessor
//    Project:   FileProcessor.Example.Moves
//    File Name: MovesRecordProcessorViewModel.cs
//  
//    Author:    Andrew - 2017/05/31
//  ---------------------------------------------

namespace FileProcessor.Example.Moves.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using Engines;
    using Models;

    public class MovesRecordProcessorViewModel : BaseViewModel
    {
        #region Properties

        private ObservableCollection<TicketPlate> _plates;

        public ObservableCollection<TicketPlate> Plates
        {
            get
            {
                return _plates;
            }
            set
            {
                _plates = value;

                SetCommandAvailability();
                
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RegisteredOwner> _registeredOwners;

        public ObservableCollection<RegisteredOwner> RegisteredOwners
        {
            get
            {
                return _registeredOwners;
            }

            set
            {
                _registeredOwners = value;

                OnPropertyChanged();
            }
        }

        #endregion

        public MovesRecordProcessorViewModel()
        {
            WireCommands();
        }

        #region Commanding

        public RelayCommand GenerateResponseRecordsCommand { get; private set; }

        private void WireCommands()
        {
            GenerateResponseRecordsCommand = new RelayCommand(GenerateResponseRecords);

            SetCommandAvailability();
        }

        private void SetCommandAvailability()
        {
            GenerateResponseRecordsCommand.IsEnabled = Plates != null && Plates.Any();
        }

        #endregion

        private void GenerateResponseRecords()
        {
            RegisteredOwners = new ObservableCollection<RegisteredOwner>();

            if (Plates == null || !Plates.Any())
                return;

            foreach (var plate in Plates)
            {
                RegisteredOwners.Add(new RegisteredOwner
                                     {
                                         PlateNumber = plate.PlateNumber,
                                         TicketNumber = plate.TicketNumber,
                                         Make = "BMW",
                                         Model = "i8",
                                         VIN = "12345678901234567",
                                         Style = "Coupe",
                                         Gender = 'M',
                                         Status = "Active",
                                         Colour = "Silver",
                                         OwnersName = "Jones",
                                         FirstName = "Bob",
                                         MiddleName = "Fred",
                                         DateOfBirth = DateTime.Today,
                                         PostalAddress = new Address
                                                         {
                                                             AddressLine1 = "12323 - 45 street",
                                                             AddressLine2 = "Apt 21",
                                                             City = "Edmonton",
                                                             Province = "AB",
                                                             Country = "CA",
                                                             PostalCode = "T6W 1T4"
                                                         },
                                         PhysicalAddress = new Address
                                                           {
                                                               AddressLine1 = "12323 - 45 street",
                                                               AddressLine2 = "Apt 21",
                                                               City = "Edmonton",
                                                               Province = "AB",
                                                               Country = "CA",
                                                               PostalCode = "T6W 1T4"
                                                           },
                                         MVID = "1234-56789",
                                         RegistrationYear = 2017,
                                         VehicleYear = 2001
                                     });
            }
        }

        public void LoadFile(string fileName)
        {
            var processor = new FileProcessor<TicketPlate>(fileName);
            Plates = new ObservableCollection<TicketPlate>(processor.ReadFile());
        }

        public void WriteFile(string fileName)
        {
            var processor = new FileProcessor<RegisteredOwner>(fileName);
            processor.WriteFile(RegisteredOwners);
        }
    }
}