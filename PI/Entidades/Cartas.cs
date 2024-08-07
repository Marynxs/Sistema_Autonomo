﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SistemaAutonomo.Entidades
{
    public class Cartas
    {
        public Dictionary<int,Carta> cartas;

        public Cartas()
        {
            cartas = new Dictionary<int, Carta>();
        }

        public void AdicionarCarta(char naipe,int idCarta)
        {
            cartas.Add(idCarta, new Carta(naipe, idCarta));
        }
    }
}
