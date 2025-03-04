using System;
using System.Collections.Generic;
using Fan_igen;
using Finns_i_Monogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Version4_finns_i_sjon;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private Texture2D pixel;
    private List<string> Kortlek = new List<string>{
        "Ess", "Ess", "Ess", "Ess", "2", "2", "2", "2", "3", "3", "3", "3",
        "4", "4", "4", "4", "5", "5", "5", "5", "6", "6", "6", "6",
        "7", "7", "7", "7", "8", "8", "8", "8", "9", "9", "9", "9",
        "10", "10", "10", "10", "Knäkt", "Knäkt", "Knäkt", "Knäkt",
        "Dam", "Dam", "Dam", "Dam", "Kung", "Kung", "Kung", "Kung"
    };
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
        // Skapa en 1x1 vit textur
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    private Random ran = new Random();
    private bool Start = true;
    private List<Spelare> SL = new List<Spelare>();
    private List<Kortvisuel> Sjön = new List<Kortvisuel>();
    private int Part_a = 1;
    private int Part_b = 1;
    private int Part_c = 1;

    // för spelare 1.
    private int KortSomSkaUp = 0;
    private int VilkenSpelare = 1;
    private Rectangle SPV = new Rectangle();
    private bool Space = true;

    // för animation
    private int anix;
    private int aniy;

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

            TestaSpelareOchSjön(SL,Sjön); // För att se så det inte är för många kort.
        }
        KeyboardState kstate = Keyboard.GetState();
        MouseState mstate = Mouse.GetState();
        if(Part_a==1){// Du gör dina saker.
            if(SL[0].hand.Count<1){
                Part_a=2; // skickar så botarna kör för din hand är tom
                Part_b=1;
                Part_c=1;
            }
            if(Part_b==1){// Du väljer kort.
                if(KortSomSkaUp>SL[0].hand.Count-1){
                    KortSomSkaUp=SL[0].hand.Count-1;// kan ge index out of range om jag inte gör detta.
                }
                if(kstate.IsKeyDown(Keys.Left)&&Space){// flytta vänster.
                    KortSomSkaUp--;
                    if(KortSomSkaUp<0){
                        KortSomSkaUp=SL[0].hand.Count-1;
                    }
                    Space=false;
                }
                if(kstate.IsKeyDown(Keys.Right)&&Space){// flytta höger
                    KortSomSkaUp++;
                    if(KortSomSkaUp>SL[0].hand.Count-1){
                        KortSomSkaUp=0;
                    }
                    Space=false;
                }
                if(kstate.IsKeyDown(Keys.Space)&&Space){ // du väljer ditt kort.
                    Space=false;
                    Part_b=2;
                }
                for(int i=0;i<SL[0].hand.Count;i++){//Så korten går ner till orginal position om de inte ska vara uppe.
                    if(i!=KortSomSkaUp){
                        SL[0].hand[i].FlyttaY=800;
                    }else{
                        SL[0].hand[i].FlyttaY=780;
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
                    SPV = new Rectangle(90,370,160,100);
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
                    int VadSkaHända = HarDeDetKortet(SL[VilkenSpelare].hand,SL[0].hand[KortSomSkaUp].Kort,VilkenSpelare);
                    if(VadSkaHända==0){
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
                        SL[0].PosiKort = VarÄrDetKortDuSöker(SL,SL[0].hand[KortSomSkaUp].Kort,SL[0].spelare);
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
                    for(int i=0;i<SL[SL[0].spelare].hand.Count;i++){//kollar varje kort i spelaren som du valde. det borde funka utan problem för du valde en spelare sen innan. i part c 1.
                        if(SL[SL[0].spelare].hand[i].Kort==SL[0].hand[KortSomSkaUp].Kort){
                            SL[0].hand.Add(SL[SL[0].spelare].hand[i]); // lägger till dem i din hand.
                            test.Add(i); // lägger i positionen av kortet som är lika i listan, för att sedan ta bort den.
                        }
                    }
                    foreach(int i in test){
                        SL[SL[0].spelare].hand.RemoveAt(i);
                    }
                    test.Clear();// behövs säkert inte men tar inga risker just nu.
                    // se till så de inte har mer av korten jag söker.
                    // kan göra det efter animationen kanske är bättre.
                }
            }
        }
        if(Part_a==2){
            Console.WriteLine("part 2");
            Part_a=1;
            Part_b=1;
            Part_c=1;
        }

        if(Part_a==10){ // animation.
            if(Part_b==1){ // för sjön. 
                // och här
                if(Part_c==0){
                    anix = (875-Sjön[SL[0].PosiKort].vitrektangle.X)/13;
                    aniy = (1000-Sjön[SL[0].PosiKort].vitrektangle.Y)/13;
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
                Console.WriteLine("ddd"+Part_a+Part_b+Part_c);
                if(Part_c==0){
                    anix = (875-SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.X)/13;
                    aniy = (1000-SL[SL[0].spelare].hand[SL[0].PosiKort].vitrektangle.Y)/13;
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

        if(kstate.IsKeyUp(Keys.Left)&&kstate.IsKeyUp(Keys.Right)&&kstate.IsKeyUp(Keys.Space)){ // så man bara kan klicka en gång
            Space=true;
        }
        if(kstate.IsKeyDown(Keys.Escape)){
            Exit();
        }
        base.Update(gameTime);
    }

    static int VarÄrDetKortDuSöker(List<Spelare> SL,string kort,int v){
        for(int i=0;i<SL[v].hand.Count;i++){
            if(SL[v].hand[i].Kort == kort){
                return i;
            }
        }
        return -1; // borde inte komma hit ändå.
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.LightSeaGreen);

        _spriteBatch.Begin();
        foreach(Kortvisuel k in Sjön){ // Ritar sjön.
            _spriteBatch.Draw(pixel,k.rödrektangle,Color.Black);
            _spriteBatch.Draw(pixel,k.vitrektangle,Color.Red);
        }
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
            _spriteBatch.DrawString(font,SL[1].text,new Vector2(100,380),Color.Black);
            _spriteBatch.DrawString(font,SL[1].hand.Count+"",new Vector2(130,420),Color.Black);

            _spriteBatch.DrawString(font,SL[2].text,new Vector2(840,50),Color.Black);
            _spriteBatch.DrawString(font,SL[2].hand.Count+"",new Vector2(870,90),Color.Black);
            
            _spriteBatch.DrawString(font,SL[3].text,new Vector2(1550,380),Color.Black);
            _spriteBatch.DrawString(font,SL[3].hand.Count+"",new Vector2(1580,420),Color.Black);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }













    static List<Kortvisuel> SåDetSerBraUt(List<Spelare> SL,int v){
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

    static int HarDeDetKortet(List<Kortvisuel> Hand, string Kort, int spelare){// om det kortet finns i den handen.
        foreach(Kortvisuel k in Hand){
            if(k.Kort == Kort){
                return spelare;
            }
        }
        return 0;
    }

    static void TestaSpelareOchSjön(List<Spelare> SL, List<Kortvisuel> Sjön){
        foreach(Spelare s in SL){
            Console.WriteLine("Spelare "+s.vemärdu);
            foreach(Kortvisuel k in s.hand){
                Console.Write(k.Kort + " ");
            }
            Console.WriteLine();
        }
        foreach(Kortvisuel k in Sjön){
                Console.Write(k.Kort + " ");
            }
    }
}