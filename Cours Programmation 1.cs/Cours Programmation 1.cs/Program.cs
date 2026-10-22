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
    public int drunkenness;   // equivalent d'une barre de vie mais inversee + couplee au systeme de power certaines boissons font augmenter le power
    public int lethalDrunkenness;
    public int power;
    public int money;

    public bool blurryState = false;

    // capacite recuperee apres le combat contre le chef
    public string unlockedAttack = "";

    // bonus et malus temporaire du flacon
    public int temporaryDrunkenness = 0;
    public int temporaryDrunkennessTurns = 0;

    public Player(int drunkenness = 0)
    {
        this.drunkenness = drunkenness;
        this.lethalDrunkenness = 100;
        this.power = 10;
        this.money = 100;
    }

    public void EndPlayerTurn(Game game)
    {
        if (temporaryDrunkennessTurns <= 0)
            return;

        temporaryDrunkennessTurns--;

        if (temporaryDrunkennessTurns == 0)
        {
            drunkenness -= temporaryDrunkenness;
            drunkenness = Math.Max(0, drunkenness);

            game.Write($"L'effet de la limonade se dissipe. Vous perdez {temporaryDrunkenness} points d'ivresse temporaires.");

            game.Write($"Votre ivresse est maintenant de {drunkenness}.");

            temporaryDrunkenness = 0;
            Console.WriteLine();
        }
    }
}

class Ennemi
{
    public int hp;
    public int hpMax = 200;
    public int power;
    public int moneyDrop;

    Random rng = new Random();

    private bool aExpliquePileOuFace = false;
    private bool aExpliqueVisionFloue = false;

    public Ennemi(int hp, int hpMax, int power, int moneyDrop)
    {
        this.hpMax = hpMax;
        this.hp = hp;
        this.power = power;
        this.moneyDrop = moneyDrop;
    }

    public bool InfligerDegats(int degats)
    {
        hp -= degats;

        if (hp < 0)
            hp = 0;

        return hp <= 0;
    }

    public void AttaquePileOuFace(Player player)
    {
        Console.WriteLine("Le chef lance l'attaque Lancer de bières, il envoi une salve de bouteille en espérant tomber sur des bières");
        Console.WriteLine();

        if (!aExpliquePileOuFace)
        {
            Console.WriteLine("Lancer de bières : effectue un pile ou face, s'il est gagnant inflige 5 point d'ivresse et relance un pile ou face jusqu'à le perdre");
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
            Console.WriteLine("Soupe suspecte : inflige 10 points d'ivresse et a 1 chance sur 4 d'infliger le statut Vision floue qui fait passer le prochain tour de l'adversaire");
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
    public List<string> storySave = new List<string>();
    private List<Page> pages = new List<Page>();

    private int currentPage = 0;

    public string mode = "";
    public string ending = "";

    public Game()
    {
        RegisterPages();
        UseLemonade();
    }

    private void RegisterPages()
    {
        pages.Add(new IntroPage(this)); //page 0
        pages.Add(new DrinkPage(this)); //page 1
        pages.Add(new NorbertLemonPage(this)); //page 2
        pages.Add(new LemonRefusePage(this)); //page 3
        pages.Add(new CombatChefPage(this)); //page 4
        pages.Add(new CombatServeusePage(this)); //page 5
        pages.Add(new EndingPage(this)); //page 6
    }

    public bool UseLemonade()
    {
        if (!inventory.Contains("Flacon de limonade"))
            return false;

        // alerte si le joueur s'apprete a se suicider
        if (player.drunkenness + 30 >= player.lethalDrunkenness)
        {
            Write("Attention !!  Votre ivresse est sur le point d'atteindre un niveau critique !!!");

            Write("Voulez-vous vraiment boire cette limonade ?");
            Write("1. Boire la limonade");
            Write("2. Annuler");

            int confirmation;

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out confirmation) && (confirmation == 1 || confirmation == 2))
                    break;

                Write("Choix invalide");
            }

            if (confirmation == 2)
            {
                Write("Annulation");
                Console.WriteLine();
                return false;
            }

            ending = "ivre mort";
            return true;
        }

        inventory.Remove("Flacon de limonade");

        player.power += 15;
        player.drunkenness += 30;

        player.temporaryDrunkenness = 30;
        player.temporaryDrunkennessTurns = 2;

        choices.Add("Utilisation du flacon de limonade");
        storySave.Add("Vous avez bu la limonade de Norbert.");

        Write("Vous buvez la limonade de Norbert !");
        Write("Votre puissance augmente de 15 et votre ivresse de 30 pendant les 2 prochains tours.");
        Write($"Puissance actuelle : {player.power}");
        Write($"Ivresse actuelle : {player.drunkenness} / " + $"{player.lethalDrunkenness}");

        Console.WriteLine();
        return true;
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
        storySave.Clear();

        mode = "";
        ending = "";

        pages.Clear();
        RegisterPages();

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
        game.Write("À l'approche du célébrissime bar L'Assoiffé, vous vous remémorez les risques que vous encourez en entrant dans un pareil endroit, cette nuit les morts vivants pullulent vous n'avez qu'à attendre l'aube mais surtout vous devez échapper à l'emprise de ce lieu maudit qui semble avaler les gens qui y pénètrent...");
        game.storySave.Add("À l'approche du célébrissime bar L'Assoiffé, vous vous remémorez les risques que vous encourez en entrant dans un pareil endroit, cette nuit les morts vivants pullulent vous n'avez qu'à attendre l'aube mais surtout vous devez échapper à l'emprise de ce lieu maudit qui semble avaler les gens qui y pénètrent...");
        Console.WriteLine();
        game.Write("Serveuse : Bienvenue à L'Assoiffé, souhaitez-vous une table ou simplement commander ?   1.Une table   2.Je veux juste boire jusqu'à l'aube");
        game.storySave.Add("Serveuse : Bienvenue à L'Assoiffé, souhaitez-vous une table ou simplement commander ?");
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
        game.storySave.Add("Serveuse : Que souhaitez-vous boire ?");
        Console.WriteLine();
        game.Write($"Vous avez actuellement {game.player.money} crédits.");
        game.Write($"Votre niveau d'ivresse est de {game.player.drunkenness} / {game.player.lethalDrunkenness}.");
        Console.WriteLine();

        int biere = 5;
        int cercueil = 20;
        int norbert = 10;
        int soupe = 30;

        game.Write($"1. Bière ({biere} crédits)    2. Cercueil ({cercueil} crédits)   3. Jus de Norbert ({norbert} crédits)   4. Soupe du chef ({soupe} crédits)");
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
            game.storySave.Add("Vous utilisez le bon de commande de Norbert et êtes convié en cuisine pour voir le chef");
            Console.WriteLine();
            game.GoToPage(4);
            return;
        }

        int price = choix switch
        {
            1 => biere,
            2 => cercueil,
            3 => norbert,
            4 => soupe,
            _ => -1
        };

        if (price == -1)
        {
            game.Write("Choix invalide");
            Console.WriteLine();
            Run();
            return;
        }

        if (game.player.money < price)
        {
            game.Write($"Serveuse : Vous n'avez pas assez de crédits ({game.player.money}) pour {price}.");
            Console.WriteLine();
            game.choices.Add("Pas assez d'argent");
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
                game.Write("Vous avez choisi une bière, un classique que vous vous empresserez de comander à nouveau jusqu'à ne plus tenir debout");
                game.storySave.Add("Vous avez choisi une bière, un classique que vous vous empresserez de comander à nouveau jusqu'à ne plus tenir debout");
                Console.WriteLine();
                break;

            case 2:
                game.choices.Add("Boisson : Cercueil");
                game.player.drunkenness += 30;
                game.player.power += 20;
                game.Write("Vous avez choisi le Cercueil, un breuvage qui vous fera perdre la raison et vous fera danser toute la nuit, laissez vous transporter dans un univers onirique fait des rêves des précédents consommateurs et des hallucinations les plus étranges");
                game.storySave.Add("Vous avez choisi le Cercueil, un breuvage qui vous fera perdre la raison et vous fera danser toute la nuit, laissez vous transporter dans un univers onirique fait des rêves des précédents consommateurs et des hallucinations les plus étranges");
                Console.WriteLine();
                break;

            case 3:
                game.choices.Add("Boisson : Jus de Norbert");
                game.Write("Vous avez choisi le jus de Norbert, la barman vous indique un étrange stand où se trouve un gnome vendant une limonade aux arômes subtiles");
                game.storySave.Add("Vous avez choisi le jus de Norbert, la barman vous indique un étrange stand où se trouve un gnome vendant une limonade aux arômes subtiles");
                Console.WriteLine();
                game.GoToPage(2);
                return;

            case 4:
                game.choices.Add("Boisson : Soupe du chef");
                game.Write("Vous avez choisi la soupe du chef et êtes convié en cuisine afin de le voir");
                game.storySave.Add("Vous avez choisi la soupe du chef et êtes convié en cuisine afin de le voir");
                Console.WriteLine();
                game.GoToPage(4);
                return;
        }

        game.Write($"Il vous reste {game.player.money} crédits.");
        Console.WriteLine();

        if (game.player.drunkenness >= game.player.lethalDrunkenness)
        {
            game.ending = "ivre mort";
            game.GoToPage(5);
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
        game.storySave.Add("Le gnome d'un geste de la main vous indique sa limonade");

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
        game.Write("Vous déclinez l'offre ce qui semble délier la langue du gnome   Norbert : ah mon ami, vous souhaitiez donc me rencontrer pour affaires je suppose, d'abord retournez au bar et ramenez moi une soupe, récupérez-la gratuitement via ce bon de commande");
        game.storySave.Add("Vous déclinez l'offre ce qui semble délier la langue du gnome");
        game.storySave.Add("Norbert : ah mon ami, vous souhaitiez donc me rencontrer pour affaires je suppose, d'abord retournez au bar et ramenez moi une soupe, récupérez-la gratuitement via ce bon de commande");
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
            game.storySave.Add("Norbert l'air déçu s'en va laissant son stand derrière lui, vous récupérez l'une de ses limonades imaginant qu'elle pourrait être utile si la soirée s'éternise");
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
    private Ennemi chef = new Ennemi(200, 200, 10, 20);
    private Random rng = new Random();

    private bool attaquePileOuFace = true;

    public CombatChefPage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Vous constatez l'état laborieux des cuisines qui, malgré votre appréhension, est pire que ce que vous imaginiez.");

        game.Write("Soudain, le chef apparaît, les yeux livides et la peau en décomposition... UN MORT-VIVANT !");

        game.Write("Chef : BEUARGHHHHHHHH !!!!!");
        game.Write("Combat engagé");
        Console.WriteLine();

        game.storySave.Add("Vous avez rencontré le chef mort-vivant dans les cuisines.");

        CombatLoop();
    }

    private void CombatLoop()
    {
        while (true)
        {
            bool tourConsomme = false;

            // Tour du joueur
            if (game.player.blurryState)
            {
                game.Write("Votre vision est floue... Vous ne pouvez pas agir ce tour.");

                game.storySave.Add("Votre vision a été troublée.");

                game.player.blurryState = false;
                tourConsomme = true;
            }
            else
            {
                int optionFlacon = game.inventory.Contains("Flacon de limonade") ? 4 : -1;

                game.Write("Que faites-vous ?");
                game.Write("1. Attaquer");
                game.Write("2. Boire de l'eau");
                game.Write("3. Fuir");

                if (optionFlacon != -1)
                    game.Write("4. Utiliser le flacon de limonade");

                int choix = AskInt();

                if (choix == 1)
                {
                    int degats = game.player.power;

                    chef.InfligerDegats(degats);

                    game.Write($"Vous attaquez le chef et lui infligez {degats} dégâts !");

                    game.Write($"Le chef possède {chef.hp} / {chef.hpMax} PV.");

                    game.storySave.Add("Vous avez attaqué le chef.");

                    tourConsomme = true;
                }
                else if (choix == 2)
                {
                    // le soin fait baisser le power
                    bool grosSoin = rng.Next(2) == 0;
                    int soin = grosSoin ? 40 : 20;

                    game.player.drunkenness -= soin;
                    game.player.power -= 5;

                    if (game.player.drunkenness < 0)
                        game.player.drunkenness = 0;

                    if (game.player.power < 0)
                        game.player.power = 0;

                    game.Write($"Vous buvez de l'eau et réduisez votre ivresse de {soin}.");

                    game.Write($"Votre puissance diminue de 5 : {game.player.power}.");

                    game.storySave.Add("Vous avez bu de l'eau.");

                    tourConsomme = true;
                }
                else if (choix == 3)
                {
                    bool fuite = rng.Next(4) != 0;

                    if (fuite)
                    {
                        game.Write("Vous fuyez le combat et retournez dans le bar.");

                        game.storySave.Add("Vous avez fui le combat.");

                        game.GoToPage(1);
                        return;
                    }

                    game.Write("Vous tentez de fuir, mais le chef vous barre la route !");

                    game.storySave.Add("Vous avez tenté de fuir le combat et avez échoué.");

                    tourConsomme = true;
                }
                else if (choix == optionFlacon && optionFlacon != -1)
                {
                    bool utilise = game.UseLemonade();

                    if (game.ending == "ivre mort")
                    {
                        game.GoToPage(6);
                        return;
                    }

                    // securite le tour n'est pas utilise
                    if (!utilise)
                        continue;

                    tourConsomme = true;
                }
                else
                {
                    game.Write("Choix invalide.");
                    Console.WriteLine();
                    continue;
                }
            }

            if (tourConsomme)
                game.player.EndPlayerTurn(game);

            // securite si le chef meurt le combat ne se poursuit pas
            if (chef.hp <= 0)
            {
                VaincreChef();
                return;
            }

            // securite si le joueur meurt le combat ne se poursuit pas
            if (game.player.drunkenness >= game.player.lethalDrunkenness)
            {
                game.ending = "ivre mort";
                game.GoToPage(6);
                return;
            }

            // Tour du chef
            if (attaquePileOuFace)
                chef.AttaquePileOuFace(game.player);
            else
                chef.AttaqueVisionFloue(game.player);

            attaquePileOuFace = !attaquePileOuFace;

            if (game.player.drunkenness >= game.player.lethalDrunkenness)
            {
                game.ending = "ivre mort";
                game.GoToPage(6);
                return;
            }
        }
    }

    private void VaincreChef()
    {
        game.player.money += chef.moneyDrop;

        game.Write("Le chef s'effondre ! Vous remportez le combat.");
        game.Write($"Vous récupérez {chef.moneyDrop} crédits. Vous possédez maintenant {game.player.money} crédits.");

        game.storySave.Add("Vous avez vaincu le chef.");

        game.Write("Le chef laisse derrière lui ses deux techniques...");
        game.Write("Laquelle souhaitez-vous récupérer ?");
        game.Write("1. Lancer de bières : effectue un pile ou face, s'il est gagnant inflige 5 point de dégât et relance un pile ou face jusqu'à le perdre");
        game.Write("2. Soupe suspecte : inflige 10 points de dégât et a 1 chance sur 4 d'infliger le statut Vision floue qui fait passer le prochain tour de l'adversaire");

        int choix;

        while (true)
        {
            choix = AskInt();

            if (choix == 1 || choix == 2)
                break;

            game.Write("Choix invalide");
        }

        if (choix == 1)
        {
            game.player.unlockedAttack = "Biere";

            game.Write("Vous apprenez Lancer de bières.");

            game.choices.Add("Capacité récupérée : Lancer de bières");
        }
        else
        {
            game.player.unlockedAttack = "Soupe";

            game.Write("Vous apprenez Soupe suspecte.");

            game.choices.Add("Capacité récupérée : Soupe suspecte");
        }

        Console.WriteLine();

        game.GoToPage(5);
    }
}


class CombatServeusePage : Page
{
    private Ennemi serveuse = new Ennemi(180, 180, 10, 0);
    private Random rng = new Random();
    private bool aExpliqueBiere = false;
    private bool aExpliqueCercueil = false;
    private bool aExpliqueImmolation = false;

    // attaque : biere a retardement
    private bool biereLancee = false;
    private int biereEnAttente = 0;

    // attaque : cercueil
    private int cumulCercueil = 0;
    private int pairsConsecutifs = 0;
    private bool immolationForcee = false;

    // attaque immolation qui devient declenchable en dessous de 50% des PV max
    private bool prochaineAttaqueImmolation = false;

    // effet de soupe suspecte
    private bool serveuseEtourdie = false;

    public CombatServeusePage(Game game) : base(game) { }

    public override void Run()
    {
        game.Write("Vous sortez des cuisines, mais la serveuse se dresse devant vous.");

        game.Write("Son visage se déforme de rage. Elle saisit plusieurs bouteilles.");

        game.Write("Serveuse : VOUS AVEZ OSÉ TOUCHER À MON CHEF ?!");

        game.Write("Combat engagé !");
        Console.WriteLine();

        game.storySave.Add("Vous affrontez la serveuse, ivre de rage.");

        CombatLoop();
    }

    private void CombatLoop()
    {
        while (true)
        {
            if (biereLancee && biereEnAttente > 0)
            {
                if (rng.Next(2) == 0)
                {
                    biereEnAttente *= 2;

                    game.Write("Réussite : La Bière à retardement s'intensifie ! Les dégâts d'ivresse en attente doublent.");
                }
                else
                {
                    game.Write("Echec : La Bière à retardement ne s'intensifie pas.");
                }

                game.player.drunkenness += biereEnAttente;

                game.Write($"Vous subissez {biereEnAttente} points d'ivresse supplémentaires.");

                game.Write($"Votre ivresse : {game.player.drunkenness} / {game.player.lethalDrunkenness}.");

                Console.WriteLine();

                if (VerifierMortJoueur())
                    return;
            }

            // Tour du joueur

            int optionCapacite = game.player.unlockedAttack != "" ? 4 : -1;

             int optionFlacon = game.inventory.Contains("Flacon de limonade") ? (optionCapacite != -1 ? 5 : 4) : -1;

             game.Write("Que faites-vous ?");
             game.Write("1. Attaquer");
             game.Write("2. Boire de l'eau");
             game.Write("3. Fuir");

             if (optionCapacite != -1)
                 game.Write($"4. Utiliser : {NomCapacite()}");

             if (optionFlacon != -1)
                 game.Write($"{optionFlacon}. Utiliser le flacon de limonade");

             int choix = AskInt();

             if (choix == 1)
             {
                 int degats = game.player.power;

                 serveuse.InfligerDegats(degats);

                 game.Write($"Vous attaquez la serveuse et infligez {degats} dégâts !");

                 game.Write($"Serveuse : {serveuse.hp} / {serveuse.hpMax} PV.");

                 game.storySave.Add("Vous avez attaqué la serveuse.");

                 game.player.EndPlayerTurn(game);
             }
             else if (choix == 2)
             {
                 bool grosSoin = rng.Next(2) == 0;
                 int soin = grosSoin ? 40 : 20;
                 game.player.drunkenness -= soin;
                 game.player.power -= 5;

                 game.player.drunkenness = Math.Max(0, game.player.drunkenness);

                 game.player.power = Math.Max(0, game.player.power);

                 game.Write($"Vous buvez de l'eau et réduisez votre ivresse de {soin}.");

                 game.Write($"Votre puissance est maintenant de {game.player.power}.");

                 game.storySave.Add("Vous avez bu de l'eau.");

                 game.player.EndPlayerTurn(game);
             }
             else if (choix == 3)
             {
                 bool fuite = rng.Next(4) != 0;

                 if (fuite)
                 {
                     game.Write("Vous parvenez à fuir la serveuse et retournez au bar.");

                     game.storySave.Add("Vous avez fui le combat contre la serveuse.");

                     game.GoToPage(1);
                     return;
                 }

                 game.Write("La serveuse vous barre la route et vous empêche de fuir !");

                 game.storySave.Add("Votre tentative de fuite a échoué.");

                 game.player.EndPlayerTurn(game);
             }
             else if (choix == optionCapacite && optionCapacite != -1)
             {
                 UtiliserCapacite();

                 game.player.EndPlayerTurn(game);
             }
             else if (choix == optionFlacon && optionFlacon != -1)
             {
                bool utilise = game.UseLemonade();

                if (game.ending == "ivre mort")
                {
                    game.GoToPage(6);
                    return;
                }

                if (!utilise)
                     continue;

                 game.player.EndPlayerTurn(game);
             }
             else
             {
                 game.Write("Choix invalide");
                 Console.WriteLine();
                 continue;
             }

            if (serveuse.hp <= 0)
            {
                VaincreServeuse();
                return;
            }

            if (VerifierMortJoueur())
                return;

            // Tour de la serveuse
            if (serveuseEtourdie)
            {
                game.Write("La serveuse titube, incapable d'agir, son tour est passé.");

                serveuseEtourdie = false;
            }
            else
            {
                AttaqueServeuse();

                if (VerifierMortJoueur())
                    return;

                if (serveuse.hp <= 0)
                {
                    VaincreServeuse();
                    return;
                }
            }
        }
    }

    private string NomCapacite()
    {
        return game.player.unlockedAttack == "Biere" ? "Lancer de bières" : "Soupe suspecte";
    }

    private void UtiliserCapacite()
    {
        if (game.player.unlockedAttack == "Biere")
        {
            game.Write("Vous lancez une salve de bières sur la serveuse !");

            while (true)
            {
                bool reussite = rng.Next(2) == 0;

                if (!reussite)
                {
                    game.Write("Face ! Votre série s'interrompt.");

                    break;
                }

                int degats = game.player.power;

                serveuse.InfligerDegats(degats);

                game.Write($"Pile ! Vous infligez {degats} dégâts à la serveuse.");

                if (serveuse.hp <= 0)
                    break;
            }
        }
        else if (game.player.unlockedAttack == "Soupe")
        {
            int degats = game.player.power;

            serveuse.InfligerDegats(degats);

            game.Write($"Vous envoyez une soupe suspecte : {degats} dégâts !");

            if (rng.Next(4) == 0)
            {
                serveuseEtourdie = true;

                game.Write("Réussite : La serveuse est étourdie !");
            }
            else
            {
                game.Write("Echec : La serveuse encaisse l'attaque et se prépare à riposter.");
            }
        }

        game.storySave.Add($"Vous avez utilisé la capacité : {NomCapacite()}.");

        Console.WriteLine();
    }

    private void AttaqueServeuse()
    {
        // premiere attaque : Biere a retardement
        if (!biereLancee)
        {
            AttaqueBiereRetardement();
            return;
        }

        // si les cumuls du cercueil sont annules alors l'immolation est forcee
        if (immolationForcee)
        {
            immolationForcee = false;
            prochaineAttaqueImmolation = false;

            AttaqueImmolation();
            return;
        }

        // rotation entre cercueil et immolation si les PV sont sous 50% des PV max
        if (serveuse.hp < serveuse.hpMax * 0.5f)
        {
            if (prochaineAttaqueImmolation)
            {
                prochaineAttaqueImmolation = false;
                AttaqueImmolation();
            }
            else
            {
                prochaineAttaqueImmolation = true;
                AttaqueCercueil();
            }

            return;
        }

        AttaqueCercueil();
    }

    private void AttaqueBiereRetardement()
    {
        game.Write("Serveuse : ATTRAPEZ ÇA !");

        game.Write("La serveuse lance l'attaque Bière à retardement, elle envoie une bouteille semblant instable.");

        if (!aExpliqueBiere)
        {
            game.Write("Bière à retardement : inflige 5 points d'ivresse au lancement. Au début de chaque tour du joueur, le montant en attente a une chance sur deux de doubler avant d'être infligé.");

            aExpliqueBiere = true;
        }

        biereLancee = true;
        biereEnAttente = 5;

        game.player.drunkenness += 5;

        game.Write("La bouteille éclate ! Votre ivresse augmente de 5.");

        game.Write($"Votre ivresse : {game.player.drunkenness} / {game.player.lethalDrunkenness}.");

        Console.WriteLine();
    }

    private void AttaqueCercueil()
    {
        game.Write("Serveuse : C'EST L'HEURE DU CERCUEIL");

        game.Write("La serveuse lance l'attaque Cercueil, elle attrape tous les restes d'alcool qu'elle trouve pour en faire un cocktail explosif.");

        if (!aExpliqueCercueil)
        {
            game.Write("Cercueil : génère un montant aléatoire entre 5 et 20. Ce montant s'ajoute au cumul des utilisations précédentes. Trois résultats pairs consécutifs réinitialisent le cumul à 5 et imposent une Immolation au tour suivant.");

            aExpliqueCercueil = true;
        }

        int resultat = rng.Next(5, 21);

        cumulCercueil += resultat;

        if (resultat % 2 == 0)
            pairsConsecutifs++;
        else
            pairsConsecutifs = 0;

        game.player.drunkenness += cumulCercueil;

        game.Write($"Réussite : La serveuse ajoute un nouvel alcool à son Cercueil, ajoute {resultat} au cumul.");

        game.Write($"Vous subissez {cumulCercueil} points d'ivresse.");

        game.Write($"Votre ivresse : {game.player.drunkenness} / {game.player.lethalDrunkenness}.");

        if (pairsConsecutifs >= 3)
        {
            game.Write("Echec : La serveuse ajoute de l'eau qui vient dillué le Cercueil.");

            game.Write("Son cumul repart à 5. La serveuse ivre de rage prépare une Immolation !");

            cumulCercueil = 5;
            pairsConsecutifs = 0;
            immolationForcee = true;
        }

        Console.WriteLine();
    }

    private void AttaqueImmolation()
    {
        game.Write("Serveuse : BRÛLEZ AVEC MOI !");

        if (!aExpliqueImmolation)
        {
            game.Write("Immolation : la serveuse sacrifie la moitié de ses PV actuels et inflige cette même quantité d'ivresse au joueur.");

            aExpliqueImmolation = true;
        }

        int sacrifice = Math.Max(1, serveuse.hp / 2);

        serveuse.hp -= sacrifice;
        serveuse.hp = Math.Max(0, serveuse.hp);

        game.player.drunkenness += sacrifice;

        game.Write($"La serveuse sacrifie {sacrifice} PV !");

        game.Write($"Elle possède maintenant {serveuse.hp} / {serveuse.hpMax} PV.");

        game.Write(
            $"Vous subissez {sacrifice} points d'ivresse."
        );

        game.Write($"Votre ivresse : {game.player.drunkenness} / {game.player.lethalDrunkenness}.");

        Console.WriteLine();
    }

    private bool VerifierMortJoueur()
    {
        if (game.player.drunkenness < game.player.lethalDrunkenness)
            return false;

        game.ending = "ivre mort";
        game.GoToPage(6);

        return true;
    }

    private void VaincreServeuse()
    {
        game.Write("La serveuse s'effondre, incapable de poursuivre le combat.");

        game.storySave.Add("Vous avez vaincu la serveuse.");

        game.ending = "serveuse vaincue";
        game.GoToPage(6);
    }
}


class EndingPage : Page
{
    public EndingPage(Game game) : base(game) { }

    public override void Run()
    {
        switch (game.ending)
        {
            case "ivre mort":
                game.Write("Votre ivresse atteint un niveau critique... Vous perdez connaissance");
                Console.WriteLine();
                break;

            case "serveuse vaincue":
                game.Write("La serveuse s'effondre. Le bar retrouve enfin son calme...");
                game.Write("Vous avez survécu à une interminable nuit.");
                Console.WriteLine();
                break;
        }
            game.Write("Fin");
            game.Write("Résumé :");
            Console.WriteLine();

        foreach (string sentence in game.storySave)
            {
                game.Write("- " + sentence);
                Console.WriteLine();
            }

        Console.WriteLine();
        game.Write($"Crédits restants : {game.player.money}");
        game.Write($"Puissance finale : {game.player.power}");
        game.Write($"Ivresse finale : {game.player.drunkenness} / {game.player.lethalDrunkenness}");
        Console.WriteLine();

        game.Write("1. Recommencer");
        int choixFin = AskInt();

        if (choixFin == 1)
        {
            game.Restart();
        }
    }
}
