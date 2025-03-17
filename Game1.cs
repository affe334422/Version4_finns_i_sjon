using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using Fan_igen;
using Finns_i_Monogame;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Version4_finns_i_sjon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private SpriteFont font2;
    private Texture2D pixel;
    private List<string> Kortlek = new List<string>{
        "Ess", "Ess", "Ess", "Ess", "2", "2", "2", "2", "3", "3", "3", "3",
        "4", "4", "4", "4", "5", "5", "5", "5", "6", "6", "6", "6",
        "7", "7", "7", "7", "8", "8", "8", "8", "9", "9", "9", "9",
        "10", "10", "10", "10", "Knäkt", "Knäkt", "Knäkt", "Knäkt",
        "Dam", "Dam", "Dam", "Dam", "Kung", "Kung", "Kung", "Kung"
    };
    private List<string> Namn = new List<string>{
        "Texas","Alfred","Edvin","Kurt","Johan","Kent","Bert","Gunbrit","Olof","Åsa","Saga",
        "Sosé","Abbe","Momme","José","Bära","Per","Olle","Sten","Sofia","Maja","Vivi","Benny"
        ,"Dale","Bubben","Amir","Abdi","Brita","Jesus"
    };
    private Random ran = new Random();
    private bool Start = true;
    private List<Spelare> SL = new List<Spelare>();
    private List<Kortvisuel> Sjön = new List<Kortvisuel>();
    private int Part_a = 1;
    private int Part_b = 1;
    private int Part_c = 1;
    private int[] VilkaVinner = [0,1,2,3];
    bool ÄrDeFärdiga;

    // för spelare 1.
    
    private int VilkenSpelare = 1;
    private Rectangle SPV = new Rectangle();
    private bool Space = true;

    // för animation
    private int anix;
    private int aniy;


    // för textruta
    // kan lägga dem i klassen om jag vill för att göra att allas moves skrivs ut i en ruta.
    private bool TextRuta;//
    private int TextVSpel;//
    private int TextFrågarVem;//
    private string TextKort;//
    private int TextAntal=1;//

    /*
        för att säta "kapitel"
        För att hitta alla delar där man kan se vad de andra frågar efter.
        Den är bra att ha för när om jag ska göra en smartare bot. 
            k423    tryck change all ocurenses för att hitta den.

        för sl[0].posikort. den har avvänds lite fel för den var ingenteligen planerad att användas 
        för att visa positionen av det kort i din hand och inte i deras hand men men. bara bra att minnas.

        för att göra det med rutan som skriver vad de gör.
            k555
        k555 är för att visa var all information.
            vem som frågar.
            vem den frågar.
            vilket kort.
            och hur många de hade eller om de plockade från sjön.
        
    */
    KeyboardState kstate;
    MouseState mstate;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.PreferredBackBufferWidth = 1800;
    }
    protected override void Initialize()
    {
        base.Initialize();
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        font = Content.Load<SpriteFont>("myFont");
        font2 = Content.Load<SpriteFont>("font2");
        // Skapa en 1x1 vit textur
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    
    protected override void Update(GameTime gameTime)
    {
        if(Start){
            Start=false; 
            // fixar sjön.
            int a = Kortlek.Count; 
            for(int i=0;i<a;i++){
                Sjön.Add(new Kortvisuel(ran.Next(500,1251),ran.Next(200,601),Kortlek[i]));
            }
            foreach(Kortvisuel k in Sjön){
                k.ÄndraLängd = 80;
                k.ÄndraTjock = 50;
            }
            Kortlek.Clear();
            // fixar spelare.
            for(int i=0;i<4;i++){
                SL.Add(new Spelare(i));
            }
            foreach(Spelare s in SL){ // lägger till kort i deras händer.
                for(int i=0;i<7;i++){
                    int b = ran.Next(0,Sjön.Count);
                    s.hand.Add(Sjön[b]);
                    Sjön.RemoveAt(b);
                }
            }
            if(SL[0].hand.Count>0){// kör så du har en hand so mser okej ut.
                SL[0].hand = SåDetSerBraUt(SL,0);
            }
            foreach(Kortvisuel Kort in SL[1].hand){ // flyttar korten så de är i deras del av spela planen.
                Kort.FlyttaX = 110;
                Kort.FlyttaY = 450;
                Kort.ÄndraLängd = 80;
                Kort.ÄndraTjock = 50;
            }
            foreach(Kortvisuel Kort in SL[2].hand){
                Kort.FlyttaX = 850;
                Kort.FlyttaY = 70;
                Kort.ÄndraLängd = 80;
                Kort.ÄndraTjock = 50;
            }
            foreach(Kortvisuel Kort in SL[3].hand){
                Kort.FlyttaX = 1560;
                Kort.FlyttaY = 450;
                Kort.ÄndraLängd = 80;
                Kort.ÄndraTjock = 50;
            }
            SL[0].dittnamn="dig";
            for(int i=1;i<4;i++){
                int f = ran.Next(0,Namn.Count);
                SL[i].dittnamn=Namn[f];
                Namn.RemoveAt(f);
            }
            
            TestaSpelareOchSjön(SL,Sjön); // För att se så det inte är för många kort.
        }
        kstate = Keyboard.GetState();
        mstate = Mouse.GetState();
        if(Part_a==1){// Du gör dina saker.
            Spelare();
        }
        if(Part_a==2){
            ÄrDeFärdiga = false; // för att botarna ska kunna köra igen.
            // ska göra allt med botarna.
            if(Part_b==1){//bot nummer 1? eller alla i en. ta kort från spelare eller från sjön. Med animation kanske.
                Botar(1);
            }
            if(Part_b==2){//samma som part b = 1 men med annan bot så när jag updaterat nästa så är det bara att göra samma sak här.
                Botar(2);
            }
            if(Part_b==3){
                Botar(3);
            }
            if(ÄrDeFärdiga){ // om bot 3 tog från sjön så blir det din tur igen.
                Part_a=1;
                Part_b=1;
                Part_c=1;
            }
            
        }
        KollaAlla4(SL);//ta bort de kort som är fyra.
        VilkaVinner = Vinner(VilkaVinner,SL); // kollar vem som leder.
        if(SL[0].hand.Count<1&&SL[1].hand.Count<1&&SL[2].hand.Count<1&&SL[3].hand.Count<1&&Sjön.Count<1){
            Part_a=12;// alla kort är borta så någon van.
        }
        if(Part_a==10){ // animation. för spelare. gör en ny för botarna.
            if(Part_b==1){ // för sjön. 
                // och här
                if(Part_c==0){
                    anix = (875-Sjön[SL[0].PosiKort].vitrektangle.X)/20;
                    aniy = (1000-Sjön[SL[0].PosiKort].vitrektangle.Y)/20;
                    Part_c=1;// för att starta animationen.
                }
                if(Part_c==1){
                    Sjön[SL[0].PosiKort].FlyttaX = Sjön[SL[0].PosiKort].vitrektangle.X+anix;
                    if(Sjön[SL[0].PosiKort].vitrektangle.Y < 920){
                        Sjön[SL[0].PosiKort].FlyttaY = Sjön[SL[0].PosiKort].vitrektangle.Y+aniy;
                    }else{
                        Part_a = 2; // skickar till botarnas tur.
                        Part_b = 1;
                        Part_c = 1;
                        SL[0].hand.Add(Sjön[SL[0].PosiKort]);
                        Sjön.RemoveAt(SL[0].PosiKort);
                        SL[0].PosiKort=-1;// för då om den används när den inte borde så blir det index out of range.
                        SL[0].hand = SåDetSerBraUt(SL,0);
                        // behöver så handen blir sorterad för att korten inte ska vara små och på fel plats.
                    }
                }
            }
            if(Part_b==2){// för bot 1.
                if(Part_c==0){
                    anix = (875-SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.X)/20;
                    aniy = (1000-SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.Y)/20;
                    Part_c=1;
                }
                if(Part_c==1){ // animationen.
                    SL[SL[0].spelare].hand[SL[0].PosiKort].FlyttaX = SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.X+anix;
                    if(SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.Y<920){
                        SL[SL[0].spelare].hand[SL[0].PosiKort].FlyttaY = SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.Y+aniy;
                    }else{
                        Part_a=1; // kan göra så den tar alla kort här eller så skickar den det till.
                        Part_b=3;
                        Part_c=2;
                        SL[0].hand.Add(SL[SL[0].spelare].hand[SL[0].PosiKort]);
                        SL[SL[0].spelare].hand.RemoveAt(SL[0].PosiKort);
                        SL[0].PosiKort=-1;
                        SL[0].hand = SåDetSerBraUt(SL,0);
                    }
                }
            }
            // om jag ska göra animation för rästen.
        }
        if(Part_a==21){ // 21 & 22 & 23. 2 an är för att det inte ska riskira att användas fel och 1-3 för att visa vilken det är.
            // jag ska använda dem för att skriva vad som händer i en ruta där man trycker space för att gå vidare.
            /*
                kanske bara behöver en med
                bool om de tar från sjön eller inte.
                int vilken spelare.
                int vem han frågar.
                string vilket kort.
                int hur många de hade.
                    eller om du inte hade något och tog från sjön istället.
                int vad part b va och ska bli.

                text:
                    if(bool true){
                        Spelare (x) frågade spelare (y) efter (kort) och de hade (antal) st.
                    }else{
                        Spelare (x) frågade spelare (y) efter (kort) och de hade 0 st, så spelare (x) plockade från sjön.
                    }
                    
            */
            if(Part_c==1){ // bot 1 tog från sjön
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=2;
                    Part_b=2;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
            if(Part_c==2){
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=2;
                    Part_b=1;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
        }
        if(Part_a==22){ // k555 vilken spelare som frågar får vi hav 2 1 eler 3 i 21,22,23.
            if(Part_c==1){ // bot 2 tog från sjön
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=2;
                    Part_b=3;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
            if(Part_c==2){
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=2;
                    Part_b=2;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
        }
        if(Part_a==23){
            if(Part_c==1){ // bot 3 tog från sjön
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=1;
                    Part_b=1;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
            if(Part_c==2){
                if(kstate.IsKeyDown(Keys.Space)&&Space){
                    Part_a=2;
                    Part_b=3;
                    Part_c=1;
                    TextAntal=0;
                    Space=false;
                    TextRuta=true;
                }
            }
        }

        if(kstate.IsKeyUp(Keys.Left)&&kstate.IsKeyUp(Keys.Right)&&kstate.IsKeyUp(Keys.Space)){ // så man bara kan klicka en gång
            Space=true;
        }
        if(kstate.IsKeyDown(Keys.Escape)){
            Exit();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSeaGreen);

        _spriteBatch.Begin();
        if(Part_a!=12){
            foreach(Kortvisuel k in Sjön){ // Ritar sjön.
                _spriteBatch.Draw(pixel,k.rödrektangle,Color.Black);
                _spriteBatch.Draw(pixel,k.vitrektangle,Color.Red);
            }
            // här ska vi ha så den ritat upp vad botarna gör.
            foreach(Kortvisuel Kort in SL[0].hand){ // ritar din hand.
                _spriteBatch.Draw(pixel,Kort.rödrektangle,Color.Red);
                _spriteBatch.Draw(pixel,Kort.vitrektangle,Color.White);
                _spriteBatch.DrawString(font,Kort.Kort,new Vector2(Kort.vitrektangle.X+10,Kort.vitrektangle.Y+10),Color.Black);
            }
            
            if(true){// ritar spelare och antalet kort de har och flera andra saker.
                if(Part_a==1&&Part_b==2&&Part_c==1){// gör en röd rektangel vid den du vill fråga.
                    _spriteBatch.Draw(pixel,SPV,Color.Red);
                }
                if(Part_a==10&&Part_b==2&&Part_c==1){ // kan kanske ändra nollan så det är en varibial på vilkens tur det är.
                    _spriteBatch.Draw(pixel,SL[SL[0].spelare].hand[SL[0].PosiKort].rödrektangle,Color.Red);
                }
                _spriteBatch.DrawString(font,SL[1].dittnamn,new Vector2(100,380),Color.Black);
                _spriteBatch.DrawString(font,SL[1].hand.Count+"",new Vector2(130,420),Color.Black);

                _spriteBatch.DrawString(font,SL[2].dittnamn,new Vector2(840,50),Color.Black);
                _spriteBatch.DrawString(font,SL[2].hand.Count+"",new Vector2(870,90),Color.Black);
                
                _spriteBatch.DrawString(font,SL[3].dittnamn,new Vector2(1550,380),Color.Black);
                _spriteBatch.DrawString(font,SL[3].hand.Count+"",new Vector2(1580,420),Color.Black);
            }
            if(Part_a==21||Part_a==22||Part_a==23){ // ritar ut rutan för det alla tar och hur många de får tag i.
                /*
                    vet inte hur jag ska göra det men om jag kan spara informationen i en klass till exempel så kan jag skriva all information i en ruta istället för tre.
                */
                _spriteBatch.Draw(pixel,new Rectangle(100,100,1600,800),Color.DimGray); // jag är sugen på o ge alla ett namn.
                if(TextRuta){
                    _spriteBatch.DrawString(font2,SL[TextVSpel].dittnamn+" frågade "+SL[TextFrågarVem].dittnamn+" efter "+TextKort, new Vector2(200,200),Color.Black);
                    if(TextFrågarVem!=0){ 
                        _spriteBatch.DrawString(font2,"och de hade "+TextAntal +"st.",new Vector2(300,300),Color.Black);
                    }else{
                        _spriteBatch.DrawString(font2,"och du hade "+TextAntal +"st.",new Vector2(300,300),Color.Black);
                    }
                }else{
                    if(TextFrågarVem!=0){
                        _spriteBatch.DrawString(font2,SL[TextVSpel].dittnamn+" frågade "+SL[TextFrågarVem].dittnamn+" efter "+TextKort+" och de",new Vector2(200,200),Color.Black);                                                                    
                    }else{
                        _spriteBatch.DrawString(font2,SL[TextVSpel].dittnamn+" frågade "+SL[TextFrågarVem].dittnamn+" efter "+TextKort+" och du",new Vector2(200,200),Color.Black);                                                                    
                    }
                    _spriteBatch.DrawString(font2,"hade 0 st, så "+SL[TextVSpel].dittnamn+" plockade från sjön.",new Vector2(200,300),Color.Black);
                }
                _spriteBatch.DrawString(font,"Tryck 'Space / mellan slag' för att gå vidare.", new Vector2(500,820),Color.Black);
            }
        }else{
            SL[0].dittnamn="Du";
            _spriteBatch.DrawString(font2,SL[VilkaVinner[0]].dittnamn+" kom först med "+SL[VilkaVinner[0]].poäng+" poäng",new Vector2(300,300),Color.Black); 
            _spriteBatch.DrawString(font2,SL[VilkaVinner[1]].dittnamn+" kom tvåa med "+SL[VilkaVinner[1]].poäng+" poäng",new Vector2(300,400),Color.Black); 
            _spriteBatch.DrawString(font2,SL[VilkaVinner[2]].dittnamn+" kom trea med "+SL[VilkaVinner[2]].poäng+" poäng",new Vector2(300,500),Color.Black); 
            _spriteBatch.DrawString(font2,SL[VilkaVinner[3]].dittnamn+" kom sist med "+SL[VilkaVinner[3]].poäng+" poäng",new Vector2(300,600),Color.Black); 
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }









    void Spelare(){
        if(SL[0].hand.Count<1&&Sjön.Count<1){
            Part_a=2; // skickar så botarna kör för din hand är tom
            Part_b=1;
            Part_c=1;
        }else{
            if(Part_b==1){// Du väljer kort.
                if(SL[0].hand.Count<1){
                    SL[0].hand.Add(Sjön[ran.Next(0,Sjön.Count)]); // göra det med resten.
                }
                SL[0].hand=SåDetSerBraUt(SL,0);
                if(SL[0].KortSomSkaUp>SL[0].hand.Count-1){
                    SL[0].KortSomSkaUp=SL[0].hand.Count-1;// kan ge index out of range om jag inte gör detta.
                }
                if(kstate.IsKeyDown(Keys.Left)&&Space){// flytta vänster.
                    SL[0].KortSomSkaUp--;
                    if(SL[0].KortSomSkaUp<0){
                        SL[0].KortSomSkaUp=SL[0].hand.Count-1;
                    }
                    Space=false;
                }
                if(kstate.IsKeyDown(Keys.Right)&&Space){// flytta höger
                    SL[0].KortSomSkaUp++;
                    if(SL[0].KortSomSkaUp>SL[0].hand.Count-1){
                        SL[0].KortSomSkaUp=0;
                    }
                    Space=false;
                }
                if(kstate.IsKeyDown(Keys.Space)&&Space){ // du väljer ditt kort.
                    Space=false;
                    // k423
                    Part_b=2;
                }
                for(int i=0;i<SL[0].hand.Count;i++){//Så korten går ner till orginal position om de inte ska vara uppe.
                    if(i!=SL[0].KortSomSkaUp){
                        SL[0].hand[i].FlyttaY=800;
                    }else{
                        SL[0].hand[i].FlyttaY=750;
                    }
                }    
            }
        }
        if(Part_b==2){// Du väljer spelare.
            if(kstate.IsKeyDown(Keys.Left)&&Space){
                VilkenSpelare--;
                if(VilkenSpelare<1){
                    VilkenSpelare=3;
                }
                Space=false;
            } 
            if(kstate.IsKeyDown(Keys.Right)&&Space){
                VilkenSpelare++;
                if(VilkenSpelare>3){
                    VilkenSpelare=1;
                }
                Space=false;
            }
            if(VilkenSpelare==1){
                SPV = new Rectangle(90,370,160,100);// gör en röd rektangel bakom den du väljer.
            }if(VilkenSpelare==2){
                SPV = new Rectangle(830,40,160,100);
            }if(VilkenSpelare==3){
                SPV = new Rectangle(1540,370,160,100);
            }
            if(kstate.IsKeyDown(Keys.Space)&&Space){
                Part_b=3;
                Space=false;
            }  
        }
        if(Part_b==3){// du tar kort från den spelaren. eller tar från sjön.
            if(Part_c==1){// för den ska till baka hit men för att se till så de inte har mer kort. i part c == 2.
                int VadSkaHända = HarDeDetKortet(SL[VilkenSpelare].hand,SL[0].hand[SL[0].KortSomSkaUp].Kort,VilkenSpelare);
                if(VadSkaHända==-1){
                    if(Sjön.Count>0){ // ta från sjön med animation.
                        SL[0].PosiKort=ran.Next(0,Sjön.Count);
                        Part_a=10;// skickar till animation.
                        Part_b=1;
                        Part_c=0;
                    }
                    else{
                        Part_a=2; // part 2 utan att ta från sjön.
                        Part_b=1;
                        Part_c=1;
                    }
                }
                else{
                    // du ska ta från botarna.
                    SL[0].spelare = VadSkaHända;// du får till baka vilken spelare du vill fråga.
                    SL[0].PosiKort = VarÄrDetKortDuSöker(SL,SL[0].hand[SL[0].KortSomSkaUp].Kort,SL[0].spelare);
                    Part_a=10; // skickar till animation.
                    Part_b=2;
                    Part_c=0; 
                }
            }
            if(Part_c==2){
                // om jag vill att den ska göra animation varje gång så är det bra att ha dennna annars skulle jag kunna ha den i part 10 animation för denna.
                Part_a=1;// skickar tillbaka till början för du tog ett kort.
                Part_b=1;
                Part_c=1;
                List<int> test = new List<int>();
                for(int i=SL[SL[0].spelare].hand.Count-1;i>0;i--){//kollar varje kort i spelaren som du valde. det borde funka utan problem för du valde en spelare sen innan. i part c 1.
                    if(ÄrDetLikaKort(SL,0,i)){
                        SL[0].hand.Add(SL[SL[0].spelare].hand[i]); // lägger till dem i din hand.
                        test.Add(i); // lägger i positionen av kortet som är lika i listan, för att sedan ta bort den.
                    }
                }                    
                foreach(int i in test){// börjar ta från slutet av handen för att inte flytta och ta fel kort.
                    SL[SL[0].spelare].hand.RemoveAt(i);
                }
                test.Clear();// behövs säkert inte men tar inga risker just nu.
                // se till så de inte har mer av korten jag söker.
                // kan göra det efter animationen kanske är bättre.
            }
        }
    }
    void Botar(int VilkenBot){
        if(SL[VilkenBot].hand.Count<1&&Sjön.Count>0){
            SL[VilkenBot].hand.Add(Sjön[ran.Next(0,Sjön.Count)]);
        }
        if(SL[VilkenBot].hand.Count>0){
            SL[VilkenBot].PosiKort=Bot_VäljerKort(SL,VilkenBot); 
            SL[VilkenBot].spelare=Bot_VäljerSpelare(SL,VilkenBot);
            TextFrågarVem=SL[VilkenBot].spelare;//k555
            TextVSpel=Part_b;//k555
            TextKort=SL[VilkenBot].hand[SL[VilkenBot].PosiKort].Kort;
            int test = HarDeDetKortet(SL[SL[VilkenBot].spelare].hand,SL[VilkenBot].hand[SL[VilkenBot].PosiKort].Kort,SL[VilkenBot].spelare);
            if(test==-1){
                if(Sjön.Count>0){
                    int a = ran.Next(0,Sjön.Count);
                    SL[VilkenBot].hand.Add(Sjön[a]);
                    Sjön.RemoveAt(a);
                    TextRuta=false;
                    Part_a=20+VilkenBot; // skicka för att göra text rutan. för att skriva att bot 1 tog från sjön.
                    Part_b=-1;
                    Part_c=1;
                }else{
                    Part_b=VilkenBot+1;
                    if(Part_b==4){
                        ÄrDeFärdiga=true;
                    }
                }
            }else{
                for(int ii=0;ii<SL[SL[VilkenBot].spelare].hand.Count;ii++){
                    if(ÄrDetLikaKort(SL,VilkenBot,ii)){
                        SL[VilkenBot].hand.Add(SL[SL[VilkenBot].spelare].hand[ii]);
                        SL[SL[VilkenBot].spelare].hand.RemoveAt(ii);
                        TextAntal++;
                        ii--;
                    }
                }
                TextRuta=true;
                Part_a=20+VilkenBot;
                Part_b=-1;
                Part_c=2;
            }
        }else{
            Part_b=VilkenBot+1;
            if(Part_b==4){
                ÄrDeFärdiga=true;
            }
        }
    }
    
    
    
    static bool ÄrDetLikaKort(List<Spelare> SL,int DU,int PositionAvKort){
        if(SL[SL[DU].spelare].hand[PositionAvKort].Kort==SL[DU].hand[SL[DU].KortSomSkaUp].Kort){
            return true;
        }
        return false;
    }
    static void KollaAlla4(List<Spelare> SL){
        for(int i = 0;i<4;i++){
            List<string> DinaKOrt = new List<string>();
            List<int> nummer = new List<int>();
            for(int a=0;a<SL[i].hand.Count;a++){
                DinaKOrt.Add(SL[i].hand[a].Kort);
            }
            string[] Kort = {"2","3","4","5","6","7","8","9","10","Knäkt","Dam","Kung","Ess"};
            foreach(string k in Kort){
                int antal = DinaKOrt.FindAll(l => l == k).Count;
                if(antal==4){
                    for(int b=SL[i].hand.Count-1;b>-1;b--){
                        if(SL[i].hand[b].Kort==k){
                            nummer.Add(b);
                        }
                    }
                    for(int kk=0;kk<4;kk++){
                        SL[i].hand.RemoveAt(nummer[kk]);
                    }
                    SL[i].poäng=1;
                }               
            }
        }
    }
    int[] Vinner(int[] vv, List<Spelare> SL){
        for(int sp=0;sp<3;sp++){
            if(SL[vv[sp]].poäng<SL[vv[sp+1]].poäng){
                int a=vv[sp];
                vv[sp] = vv[sp+1];
                vv[sp+1]=a;
            }
        }
        return vv;
    }
    static int Bot_VäljerKort(List<Spelare> SL,int v){ // väljer ett kort. kan förbättras med minnet som jag pratade om innan.
        Random ran = new Random();
        return ran.Next(0,SL[v].hand.Count);
    }
    static int Bot_VäljerSpelare(List<Spelare> SL,int v){ // samma sak här. den kan förbettras med ett minne.
        Random ran = new Random();
        int a = ran.Next(0,4);
        if(a==v){
            return Bot_VäljerSpelare(SL,v);
        }
        if(SL[a].hand.Count<1){
            return Bot_VäljerSpelare(SL,v);
        }
        return a;
    }
    static int VarÄrDetKortDuSöker(List<Spelare> SL,string kort,int v){ // hittar positionen för det kort du söker.
        for(int i=0;i<SL[v].hand.Count;i++){
            if(SL[v].hand[i].Kort == kort){
                return i;
            }
        }
        return -1; // borde inte komma hit ändå.
    }
    static List<Kortvisuel> SåDetSerBraUt(List<Spelare> SL,int v){ // sorterar och plaserar korten så de ser bra ut.
        int Y = 800;
        int gräns = 1200;
        List<Kortvisuel> New = new List<Kortvisuel>();
        gräns = gräns/SL[v].hand.Count;
        for(int i = 0;i<SL[v].hand.Count;i++){
            New.Add(new Kortvisuel((gräns * i)+300,Y,SL[v].hand[i].Kort));
        }
        SL[v].hand.Clear();
        string[] ordning = {"2","3","4","5","6","7","8","9","10","Knäkt","Dam","Kung","Ess"};
        for(int i=0;i<New.Count;i++){
            for(int ii=0;ii<New.Count-1;ii++){
                if(Array.IndexOf(ordning,New[ii].Kort)>Array.IndexOf(ordning,New[ii+1].Kort)){
                    string a = New[ii].Kort;
                    New[ii].Kort = New[ii+1].Kort;
                    New[ii+1].Kort = a;
                }
            }
        }
        return New;
    }
    static int HarDeDetKortet(List<Kortvisuel> Hand, string Kort, int DenAndraSpelare){// om det kortet finns i den handen.
        foreach(Kortvisuel k in Hand){
            if(k.Kort == Kort){
                return DenAndraSpelare;
            }
        }
        return -1;
    }
    static void TestaSpelareOchSjön(List<Spelare> SL, List<Kortvisuel> Sjön){ // för att skriva up alla spelarnas händer och sjöns kort.
        foreach(Spelare s in SL){;
            foreach(Kortvisuel k in s.hand){
                Console.Write(k.Kort + " ");
            }
            Console.WriteLine();
        }
        foreach(Kortvisuel k in Sjön){
            Console.Write(k.Kort + " ");
        }
        Console.WriteLine();
    }
}