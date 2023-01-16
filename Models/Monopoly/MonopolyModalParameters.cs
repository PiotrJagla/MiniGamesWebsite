﻿using Enums.Monopoly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Monopoly
{
    public class MonopolyModalParameters
    {
        public ModalShow WhenShowModal { get; set; }
        public StringModalParameters Parameters { get; set; }

        public ModalResponseIdentifier Identifier { get; set; }

        public MonopolyModalParameters(
            StringModalParameters Parameters, ModalShow WhenShowModal, ModalResponseIdentifier identifier)
        {
            this.Parameters = Parameters;
            this.WhenShowModal = WhenShowModal;
            this.Identifier = identifier;
        }
    }
}
