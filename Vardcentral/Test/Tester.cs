using System.Diagnostics;
using Vardcentral.Domain;
using Vardcentral.Domain.Enums;
using Vardcentral.Application.Services;
using Vardcentral.Application.Strategier;
using Vardcentral.Infrastructure;
namespace Vardcentral.Tests;

public static class Tester
{
  public static void KörAlla() 
  {
    Överlappar_KrockOchMöts();
 Tidslucka_SlutFöreStart_KastarFel();
 Patient_FelPersonnummer_KastarFel();
 Personal_FelId_KastarFel();
 Boka_SammaPersonalSammaTid_Hindras();
 Boka_SammaRumSammaTid_Hindras();
 Boka_FelPersonaltyp_Hindras();
 Sök_BokadTidVisasInte();
 Sök_Lördag_GerTomLista();
 Avboka_GörTidenLedigIgen();
 StällIn_FlyttasTillAndraVårdcentralen();
 StällIn_IngenErsättare_BlirInställd();
 Sortering_AkutFörst();
 Console.WriteLine("Alla tester klara.");

 }
 private static void Överlappar_KrockOchMöts()
 {
    var dag = new DateTime(2026, 11, 2);
 var nio_tio = new Tidslucka(dag.AddHours(9), dag.AddHours(10));
 var halvtio_halvelva = new Tidslucka(dag.AddHours(9.5), dag.AddHours(10.5));
 var tio_elva = new Tidslucka(dag.AddHours(10), dag.AddHours(11));

 Debug.Assert(nio_tio.Överlappar(halvtio_halvelva), "Delvis samma tid ska krocka");
 Debug.Assert(!nio_tio.Överlappar(tio_elva), "Möts (slut = start) ska INTE krocka");
 Debug.Assert(!tio_elva.Överlappar(nio_tio), "Samma sak åt andra hållet");

 }

 private static void Tidslucka_SlutFöreStart_KastarFel()
 {
  var tid = new DateTime(2026, 11, 2, 9, 0, 0);
  bool kastade = false;
  try { new Tidslucka(tid, tid); }
  catch (ArgumentException) { kastade = true; }
  Debug.Assert(kastade, "Slut <= start ska ge ArgumentException");

 }
 
 private static void Patient_FelPersonnummer_KastarFel()
 {
    bool kastade = false;
    try { new Patient("Test", "Testsson", "12345"); } // för kort
    catch (ArgumentException) { kastade = true; }
    Debug.Assert(kastade, "Personnummer med fel längd ska ge ArgumentException");

    var ok = new Patient("Test", "Testsson", "0001011234");
    Debug.Assert(ok.Personnummer == "0001011234", "Rätt personnummer ska sparas");

 }
 
 private static void Personal_FelId_KastarFel()
 {
  
    var vc = new Vårdcentral(1, "Test");
    var rum = new Rum(1, "Rum 1");
    bool kastade = false;
    try { new Personal("Test", "Testsson", Personaltyp.Läkare, "12a456", vc, rum); } // bokstav
    catch (ArgumentException) { kastade = true; }
    Debug.Assert(kastade, "Id som inte är 6 siffror ska ge ArgumentException");

  
 }

 private static void Boka_SammaPersonalSammaTid_Hindras()
 {

   var s = new Testsystem();
   var första = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 0, 10), s.Patient1);
   var andra = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 5, 10), s.Patient2);

 Debug.Assert(första != null, "Första bokningen ska lyckas");
 Debug.Assert(andra == null, "Samma läkare samma tid ska hindras");

 }

 private static void Boka_SammaRumSammaTid_Hindras()
 {

 var s = new Testsystem();
 var sjuksk = s.Service.SkapaBokning(s.Sjuksköterskebesök, s.SjukskNorr, s.Tid(9, 0, 20), s.Patient1);
 var fysioKrock = s.Service.SkapaBokning(s.Fysioterapi, s.FysioNorr, s.Tid(9, 10, 45), s.Patient2);
 var fysioEfter = s.Service.SkapaBokning(s.Fysioterapi, s.FysioNorr, s.Tid(9, 20, 45), s.Patient2);

 Debug.Assert(sjuksk != null);
 Debug.Assert(fysioKrock == null, "Rummet är upptaget 09:00–09:20");
 Debug.Assert(fysioEfter != null, "09:20 är ledigt (tiderna möts bara)");

 }
 
 private static void Boka_FelPersonaltyp_Hindras()
 {
    var s = new Testsystem();
    var bokning = s.Service.SkapaBokning(s.Fysioterapi, s.LäkareNorr, s.Tid(10, 0, 45), s.Patient1);
    Debug.Assert(bokning == null, "Fysioterapi kan inte bokas hos en läkare");
 }
 
 private static void Sök_BokadTidVisasInte()
 {
    var s = new Testsystem();
    s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(8, 0, 10), s.Patient1);

    var lediga = s.Service.SökLedigaTider(s.Läkarbesök, s.Norr, s.Dag);

    Debug.Assert(lediga.Count == 1, "En läkare på Norr");
    Debug.Assert(lediga[0].Tider[0].Start == s.Dag.AddHours(8).AddMinutes(10), "08:00 är bokad, 08:05 krockar, första lediga ska vara 08:10");

 }
 
 private static void Sök_Lördag_GerTomLista()
 {
    var s = new Testsystem();
    var lördag = s.Dag.AddDays(5); // Dag är en måndag
    Debug.Assert(s.Service.SökLedigaTider(s.Läkarbesök, s.Norr, lördag).Count == 0, "Stängt på helger");
 }
 
 private static void Avboka_GörTidenLedigIgen()
 {
    var s = new Testsystem();
    var bokning = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 0, 10), s.Patient1)!;
    
    bool ok = s.Service.Avboka(bokning.Id);
    var ny = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 0, 10), s.Patient2);
 
    Debug.Assert(ok);
    Debug.Assert(bokning.Status == BokningsStatus.Avbokad);
    Debug.Assert(ny != null, "Tiden ska vara ledig igen");
    Debug.Assert(!s.Service.Avboka(bokning.Id), "Kan inte avboka två gånger");


 }
 
  private static void StällIn_FlyttasTillAndraVårdcentralen()
  {
    var s = new Testsystem();
    var bokning = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 0, 10), s.Patient1)!;

    s.Service.StällIn(bokning.Id);

    Debug.Assert(bokning.Status == BokningsStatus.Ombokad);
    Debug.Assert(bokning.Personal == s.LäkareSöder, "Ska flyttas till läkaren på Söder");
    Debug.Assert(bokning.Rum == s.LäkareSöder.Rum, "Rummet ska följa med personalen");

  }

  private static void StällIn_IngenErsättare_BlirInställd()
  {
    var s = new Testsystem();
    s.Service.SkapaBokning(s.Läkarbesök, s.LäkareSöder, s.Tid(9, 0, 10), s.Patient2); // Söder upptagen
    var bokning = s.Service.SkapaBokning(s.Läkarbesök, s.LäkareNorr, s.Tid(9, 0, 10), s.Patient1)!;

    s.Service.StällIn(bokning.Id);

    Debug.Assert(bokning.Status == BokningsStatus.Inställd);
  }
 private static void Sortering_AkutFörst()
 {
    var s = new Testsystem();
 var normal = s.Service.SkapaBokning(s.Sjuksköterskebesök, s.SjukskNorr, s.Tid(8, 0, 20), s.Patient1)!;
 var akut  = s.Service.BokaAkut(s.Akut, s.Norr, s.Dag, s.Patient2);
 
 Debug.Assert(akut != null);
 Debug.Assert(akut!.Tid.Start == s.Dag.AddHours(8).AddMinutes(20), "Första lediga är 08:20");
 
 var sorterade = s.Service.HämtaAktivaSorterade(); // AkutFörstRegel
 Debug.Assert(sorterade[0] == akut, "Akut först trots senare tid");

 var tidsordning = new TidsordningsRegeln();
 Debug.Assert(tidsordning.Jämför(normal, akut) < 0, "Med tidsordning kommer 08:00 först");

 }

 private class Testsystem
 {
   public Vårdcentral Norr = new Vårdcentral(1, "Norr");
   public Vårdcentral Söder = new Vårdcentral(2, "Söder");
 
   public Besöktyp Läkarbesök = new Besöktyp("Läkarbesök", 10, Personaltyp.Läkare);
   public Besöktyp Sjuksköterskebesök = new Besöktyp("Sjuksköterskebesök", 20, Personaltyp.Sjuksköterska);
   public Besöktyp Fysioterapi = new Besöktyp("Fysioterapi", 45, Personaltyp.Fysioterapeut);
   public Besöktyp Akut = new Besöktyp("Akut", 30, Personaltyp.Sjuksköterska);

   public Patient Patient1 = new Patient("Test", "Ett", "0001010001");
   public Patient Patient2 = new Patient("Test", "Två", "0001010002");
   
   public Personal LäkareNorr;
   public Personal SjukskNorr;
   public Personal FysioNorr;
   public Personal LäkareSöder;

   public BokningsService Service;
   public DateTime Dag; // nästa måndag, alltid i framtiden

   public Testsystem()
   {
     var rum1 = new Rum(1, "Rum 1");
     var rum2 = new Rum(2, "Rum 2"); // delas av sjuksköterska och fysioterapeut
     var rum3 = new Rum(3, "Rum 3");
     
     LäkareNorr = new Personal("Läkare", "Norr", Personaltyp.Läkare, "100001", Norr, rum1);
     SjukskNorr = new Personal("Sjuksköterska", "Norr", Personaltyp.Sjuksköterska, "100002", Norr, rum2);
     FysioNorr = new Personal("Fysio", "Norr", Personaltyp.Fysioterapeut, "100003", Norr, rum2);
     LäkareSöder = new Personal("Läkare", "Söder", Personaltyp.Läkare, "200001", Söder, rum3);

     var personal = new InMemoryPersonalRepository();
     personal.LäggTill(LäkareNorr);
     personal.LäggTill(SjukskNorr);
     personal.LäggTill(FysioNorr);
     personal.LäggTill(LäkareSöder);

     var bokningar = new InMemoryBokningRepository();
     var tidsSökare = new TidsSökare(personal, bokningar);

     Service = new BokningsService(bokningar, personal, tidsSökare,
     new NotifieringsService(), new AkutFörstRegel());

     Dag = DateTime.Today.AddDays(1);
     while (Dag.DayOfWeek != DayOfWeek.Monday)
     Dag = Dag.AddDays(1);

   }
   
   public Tidslucka Tid(int timme, int minut, int längdMinut)
   {
     DateTime start = Dag.AddHours(timme).AddMinutes(minut);
     return new Tidslucka(start, start.AddMinutes(längdMinut));
   }

  




 }




}
