using System;
using System.Collections.Generic;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Game game = new Game();
        game.Start();
    }
}
class Player
{
    public int drunkenness;   // equivalent d'une barre de vie mais inversée, est aussi un système de réputation déguisé certaines interactions ne se déclenchent que à un certains niveau d'ivresse
    public int lethalDrunkenness;
    public int power;
    public int money;

    public bool blurryState = false;

    public Player(int drunkenness = 0)
    {
        this.drunkenness = drunkenness;
        this.lethalDrunkenness = 100;
        this.power = 10;
        this.money = 100;
    }
}
class Ennemi
{
    public int hp;
    public int hpMax;
    public int power;
    public int moneyDrop;

    Random rng = new Random();

    private bool aExpliquePileOuFace = false;
    private bool aExpliqueVisionFloue = false;

    public Ennemi(int hpMax, int power, int moneyDrop)
    {
        this.hpMax = hpMax;
        this.hp = hpMax;
        this.power = power;
        this.moneyDrop = moneyDrop;
    }

    public bool InfligerDegats(int degats)
    {
        this.hp -= degats;
        this.hp -= Math.Clamp(this.hp, 0, this.hpMax);
        return (this.hp == 0);
    }

    public void AttaquePileOuFace(Player player)
    {
        Console.WriteLine("Le chef lance l'attaque Lancer de bières, il envoi une salve de bouteille en espérant tomber sur des bières");
        Console.WriteLine();

        if (!aExpliquePileOuFace)
        {
            Console.WriteLine("Lancer de bières : effectue un pile ou face, s'il est gagnant inflige 5 point de dégât et relance un pile ou face jusqu'à le perdre");
            Console.WriteLine();
            aExpliquePileOuFace = true;
        }

        while (true)
        {
            bool reussite = rng.Next(2) == 0;

            if (reussite)
            {
                player.drunkenness += 5;
                Console.WriteLine("Pile ! Votre ivresse augmente de 5...");
                Console.WriteLine($"Votre ivresse est maintenant de {player.drunkenness}.");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Face... La série s'interrompt, le chef vous observe en silence avec dépit.");
                Console.WriteLine();
                break;
            }
        }
    }
    public void AttaqueVisionFloue(Player player)
    {
        Console.WriteLine("Le chef lance l'attaque Soupe suspecte, il envoi une louche de sa soupe en direction du joueur");
        Console.WriteLine();

        if (!aExpliqueVisionFloue)
        {
            Console.WriteLine("Soupe suspecte : inflige 10 points de dégât et a 1 chance sur 4 d'infliger le statut Vision floue qui fait passer le prochain tour de l'adversaire");
            Console.WriteLine();
            aExpliqueVisionFloue = true;
        }

        player.drunkenness += 10;
        Console.WriteLine("Votre ivresse augmente de 10.");
        Console.WriteLine($"Votre ivresse est maintenant de {player.drunkenness}.");
        Console.WriteLine();

        bool infligeVisionFloue = rng.Next(4) == 0; // 1 chance sur 4 d'infliger le statut Vision floue

        if (infligeVisionFloue)
        {
            player.blurryState = true;
            Console.WriteLine("Votre vision devient floue... Vous passez le tour suivant à vous cogner partout.");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Vous parvenez à garder vos esprits... mais le combat n'est pas encore fini");
            Console.WriteLine();
        }
    }
}
abstract class Page
{
    protected Game game;

    public Page(Game game)
    {
        this.game = game;
    }

    protected int AskInt()
    {
        int v;
        while (!int.TryParse(Console.ReadLine(), out v)) { }
        return v;
    }

    public abstract void Run();
}
class Game
{
    public Player player = new Player();

    public List<string> inventory = new List<string>();
    public List<string> choices = new List<string>();
    private List<Page> pages = new List<Page>();

    private int currentPage = 0;

    public string mode = "";

    public Game()
    {
        RegisterPages();
    }

    private void RegisterPages()
    {
        pages.Add(new IntroPage(this)); //page 0
        pages.Add(new DrinkPage(this)); //page 1
        pages.Add(new NorbertLemonPage(this)); //page 2
        pages.Add(new LemonRefusePage(this)); //page 3
        pages.Add(new CombatChefPage(this)); //page 4
    }

    public void Start()
    {
        currentPage = 0;
        RunCurrentPage();
    }

    public void RunCurrentPage()
    {
        pages[currentPage].Run();
    }

    public void GoToPage(int index)
    {
        currentPage = index;
        RunCurrentPage();
    }

    public void Restart()
    {
        player = new Player();
        inventory.Clear();
        choices.Clear();
        Start();
    }

    public void Write(string text)
    {
        int width = Console.WindowWidth - 2;
        string[] mots = text.Split(' ');

        string ligne = "";

        foreach (string mot in mots)
        {
            if ((ligne + mot).Length > width)
            {
                Console.WriteLine(ligne);
                ligne = "";
            }
            ligne += mot + " ";
        }

        Console.WriteLine(ligne);
    }
}
class IntroPage : Page
{
    public IntroPage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("À l'approche du célébrissime Royal Zgueg, vous vous remémorez les risques que vous encourez en entrant dans un pareil endroit, cette nuit les morts vivants pullulent vous n'avez qu'à attendre l'aube mais surtout vous devez échapper à l'emprise de ce lieu maudit qui semble avaler les gens qui y pénètrent...");
        Console.WriteLine();
        game.Write("Serveuse : Bienvenue au Royal Zgueg, souhaitez-vous une table ou simplement commander ?   1.Une table   2.Je veux juste boire jusqu'à l'aube");
        Console.WriteLine();

        int choix = AskInt();

        if (choix == 1)
        {
            game.mode = "table";
            game.choices.Add("Choix : Table");
            game.player.money -= 20;
            game.Write($"Vous payez 20 crédits. Il vous reste {game.player.money} crédits.");
            Console.WriteLine();
        }
        else if (choix == 2)
        {
            game.mode = "boire";
            game.choices.Add("Choix : Boire");
        }
        else
        {
            game.Write("Choix invalide");
            Console.WriteLine();
            Run();
            return;
        }

        game.GoToPage(1);
    }
}

class DrinkPage : Page
{
    public DrinkPage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Serveuse : Que souhaitez-vous boire ?");
        Console.WriteLine();
        game.Write($"Vous avez actuellement {game.player.money} crédits.");
        game.Write($"Votre niveau d'ivresse est de {game.player.drunkenness} / {game.player.lethalDrunkenness}.");
        Console.WriteLine();

        int beer = 5;
        int zgueg = 20;
        int norbert = 10;
        int soupe = 30;

        game.Write($"1. Bière ({beer} crédits)    2. Royal Zgueg ({zgueg} crédits)   3. Jus de Norbert ({norbert} crédits)   4. Soupe du chef ({soupe} crédits)");
        Console.WriteLine();

        if (game.inventory.Contains("Commande de Norbert"))
        {
            game.Write("5. Utiliser le bon de commande de Norbert");
            Console.WriteLine();
        }

        int choix = AskInt();

        if (choix == 5 && game.inventory.Contains("Commande de Norbert"))
        {
            game.inventory.Remove("Commande de Norbert");
            game.choices.Add("Utilisation du bon de commande");
            game.Write("Vous utilisez le bon de commande de Norbert et êtes convié en cuisine pour voir le chef");
            Console.WriteLine();
            game.GoToPage(4);
            return;
        }

        int price = choix switch
        {
            1 => beer,
            2 => zgueg,
            3 => norbert,
            4 => soupe,
            _ => -1
        };

        if (price == -1)
        {
            game.Write("Choix invalide.");
            Console.WriteLine();
            Run();
            return;
        }

        if (game.player.money < price)
        {
            game.Write($"Serveuse : Vous n'avez pas assez de crédits ({game.player.money}) pour payer {price}.");
            Console.WriteLine();
            game.choices.Add("Choix invalide : Pas assez d'argent");
            Run();
            return;
        }

        game.player.money -= price;

        switch (choix)
        {
            case 1:
                game.choices.Add("Boisson : Bière");
                game.player.drunkenness += 10;
                game.player.power += 5;
                game.Write("Vous avez choisi une bière, un classique que vous vous empresserez de commander à nouveau jusqu'à ne plus tenir debout");
                Console.WriteLine();
                break;

            case 2:
                game.choices.Add("Boisson : Royal Zgueg");
                game.player.drunkenness += 30;
                game.player.power += 20;
                game.Write("Vous avez choisi le Royal Zgueg, un breuvage qui vous fera perdre la raison et vous fera danser toute la nuit, laissez vous transporter dans un univers onirique fait des rêves des précédents consommateurs et des hallucinations les plus étranges");
                Console.WriteLine();
                break;

            case 3:
                game.choices.Add("Boisson : Jus de Norbert");
                game.Write("Vous avez choisi le jus de Norbert, la barman vous indique un étrange stand où se trouve un gnome vendant une limonade aux arômes subtiles");
                Console.WriteLine();
                game.GoToPage(2);
                return;

            case 4:
                game.choices.Add("Boisson : Soupe du chef");
                game.Write("Vous avez choisi la soupe du chef et êtes convié en cuisine afin de le voir");
                Console.WriteLine();
                game.GoToPage(4);
                return;
        }

        game.Write($"Il vous reste {game.player.money} crédits.");
        Console.WriteLine();

        if (game.player.drunkenness >= game.player.lethalDrunkenness)
        {
            game.Write("Votre ivresse atteint un niveau critique... Vous perdez connaissance et la soirée recommence depuis le début.");
            Console.WriteLine();
            game.Restart();
            return;
        }

        Run();
    }
}

class NorbertLemonPage : Page
{
    public NorbertLemonPage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Le gnome d'un geste de la main vous indique sa limonade...   1. Tester la limonade   2. Rejeter l'offre");
        Console.WriteLine();

        int choix = AskInt();

        if (choix == 1)
        {
            game.choices.Add("Test limonade");
            game.player.drunkenness += 20;
            game.player.power += 10;
            game.GoToPage(1);
        }
        else if (choix == 2)
        {
            game.GoToPage(3);
        }
        else
        {
            game.Write("Choix invalide.");
            Console.WriteLine();
            Run();
        }
    }
}

class LemonRefusePage : Page
{
    public LemonRefusePage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Vous déclinez l'offre ce qui semble délier la langue du gnome   Norbert : ah mon ami, vous souhaitiez donc me rencontrer pour affaires je suppose, d'abord retournez au bar et ramenez moi une soupe, récupérez là gratuitement via ce bon de commande");
        game.Write("1. Accepter le marché   2. Refuser le marché");
        Console.WriteLine();

        int choix = AskInt();

        if (choix == 1)
        {
            game.choices.Add("Le marché de Norbert");
            game.inventory.Add("Commande de Norbert");
            game.GoToPage(1);
        }
        else if (choix == 2)
        {
            game.Write("Norbert l'air déçu s'en va laissant son stand derrière lui, vous récupérez l'une de ses limonades imaginant qu'elle pourrait être utile si la soirée s'éternise");
            Console.WriteLine();
            game.choices.Add("Refus du marché de Norbert");
            game.inventory.Add("Flacon de limonade");
            game.GoToPage(1);
        }
        else
        {
            game.Write("Choix invalide.");
            Console.WriteLine();
            Run();
        }
    }
}

class CombatChefPage : Page
{
    Ennemi chef = new Ennemi(50, 10, 20);
    Random rng = new Random();
    bool attaquePileOuFace = true; // alterne entre les deux attaques

    public CombatChefPage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Vous constatez l'état laborieux des cuisines qui malgré votre appréhension est pire que ce que vous imaginiez, soudain le chef apparait, les yeux livides et la peau en décomposition... UN MORT VIANT !!!");
        game.Write("Chef : BEUARGHHHHHHHH !!!!!");
        game.Write("Combat engagé");
        Console.WriteLine();

        CombatLoop();
    }

    void CombatLoop()
    {
        while (true)
        {
            // tour du joueur
            if (game.player.blurryState)
            {
                game.Write("Votre vision est floue... Vous ne pouvez pas agir ce tour.");
                Console.WriteLine();
                game.player.blurryState = false; // effet de statut
            }
            else
            {
                game.Write("Que faites-vous ?   1. Attaquer   2. Boire de l'eau   3. Fuir");
                int choix = AskInt();

                if (choix == 1)
                {
                    int dmg = game.player.power;
                    chef.InfligerDegats(dmg);
                    game.Write($"Vous attaquez le chef et lui infligez {dmg} dégâts !");
                    game.Write($"Le chef a {chef.hp} hp");
                    Console.WriteLine();
                }
                else if (choix == 2)
                {
                    bool grosSoin = rng.Next(2) == 0; // 1 chance sur 2 que le heal soit critique
                    int heal = grosSoin ? 15 : 10;
                    game.player.drunkenness -= heal;
                    if (game.player.drunkenness < 0) game.player.drunkenness = 0;

                    game.Write($"Vous buvez de l'eau et réduisez votre ivresse de {heal}.");
                    Console.WriteLine();
                }
                else if (choix == 3)
                {
                    bool fuite = rng.Next(4) != 0; // 3 chances sur 4 de parvenir à fuir le combat
                    if (fuite)
                    {
                        game.Write("Vous fuyez le combat et vous retrouvez à nouveau dans le bar");
                        Console.WriteLine();
                        game.GoToPage(1);
                        return;
                    }
                    else
                    {
                        game.Write("Vous tentez de fuir, mais le chef vous barre la route !");
                        Console.WriteLine();
                    }
                }
                else
                {
                    game.Write("Choix invalide.");
                    Console.WriteLine();
                    Run();
                }
            }

            if (chef.hp <= 0)
            {
                game.Write("Le chef s'effondre, vaincu. Vous remportez le combat !");
                Console.WriteLine();
                game.player.money += chef.moneyDrop;
                game.GoToPage(1);
                return;
            }

            // tour de l'ennemi
            if (attaquePileOuFace)
            {
                chef.AttaquePileOuFace(game.player);
            }
            else
            {
                chef.AttaqueVisionFloue(game.player);
            }

            attaquePileOuFace = !attaquePileOuFace; // le chef alterne entre ses 2 attaques

            if (game.player.drunkenness >= game.player.lethalDrunkenness)
            {
                game.Write("Votre ivresse atteint un niveau critique... Vous perdez connaissance et la soirée recommence depuis le début.");
                Console.WriteLine();
                game.Restart();
                return;
            }
        }
    }
}
