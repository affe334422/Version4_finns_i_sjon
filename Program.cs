using var game = new Version4_finns_i_sjon.Game1();

game.Run();


// till nästa
SpelarFas spelarFas = SpelarFas.DraKort;
if(spelarFas == SpelarFas.DraKort){

}
enum SpelarFas{
    DraKort,
    FrågaEfterKort,

}