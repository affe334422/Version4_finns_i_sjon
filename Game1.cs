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
                int Y = 800;
                int gräns = 1200;
                List<Kortvisuel> New = new List<Kortvisuel>();
                gräns = gräns/SL[0].hand.Count;
                for(int i = 0;i<SL[0].hand.Count;i++){
                    New.Add(new Kortvisuel((gräns * i)+300,Y,SL[0].hand[i].Kort));
                }
                SL[0].hand.Clear();
                SL[0].hand = New;
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
            if(SL[0].hand.Count>0){// det funkar inte om din han är tom.
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
                if(Part_b==3){// du tar kort från den spelaren.

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

    static void TaKortFrånSpelare(List<Spelare> SL, int DittKort, int Spelare){
        // Nu funkar den bara för 1 spelare.
        bool HarDeDittKort = false;
        foreach(Kortvisuel k in SL[Spelare].hand){
            
        }
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
        
        if(true){// ritar spelare och antalet kort de har och 1 annan sak.
            if(Part_b==2){// gör en röd rektangel vid den du vill fråga.
                _spriteBatch.Draw(pixel,SPV,Color.Red);
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