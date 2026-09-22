using System.Runtime.CompilerServices;

class Program
{
    static string age;
    static string choix;
    static string name;

    static int money = 0;
    static bool halfPrice = false;
    static bool angry = false;
    static bool upset = false;
    static bool VIP = false;
    static bool norbertRequest = false;
    static bool getLime = false;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("À l'approche du célébrissime Royal Zgueg, vous vous remémorez les risques que vous encourez en entrant dans un pareil endroit, cette nuit les morts vivants pullulent vous n'avez qu'à attendre l'aube mais surtout vous devez échapper à l'emprise de ce lieu maudit qui semble avaler les gens qui y pénètrent...");
        Console.WriteLine("Serveuse : Bienvenue au Royal Zgueg, souhaitez-vous une table ou simplement commander ? 1.Une table   2.Je veux juste boire jusqu'à l'aube");
        choix = Console.ReadLine();

        int choixInt;
        if (!int.TryParse(choix, out choixInt))
        {
            Console.WriteLine("Choix invalide");
            Main();
            return;
        }

        if (choixInt == 1)
        {
            Console.WriteLine("Serveuse : Vous avez choisi une table, je dois vérifier votre âge");
            age = Console.ReadLine();
            HandleAge(age, "table");
        }
        else if (choixInt == 2)
        {
            Console.WriteLine("Serveuse : Vous êtes au bon endroit pour ça, mais avant, quel âge avez-vous ?");
            age = Console.ReadLine();
            HandleAge(age, "boire");
        }
        else
        {
            Console.WriteLine("Choix invalide");
            Main();
        }
    }

    static void HandleAge(string age, string mode)
    {
        int ageNum = int.Parse(age);

        if (name == "Romain")
        {
            money = 200;
        }
        else if (ageNum > 27)
        {
            money = 100;
        }
        else
        {
            money = 30;
        }

        if (ageNum < 18)
        {
            if (mode == "table")
            {
                Console.WriteLine("Serveuse : T'es mineur tu rentres pas DEHORS... Vous tentez de rentrer chez vous mais finissez massacré par des morts vivants... C'était déjà pas l'idée du siècle de venir dans un endroit aussi peu fréquenté mais vous avez fait fort en donnant votre âge, l'honnêteté ne paye plus depuis longtemps dans ce monde...");
                HandleAge(age, mode);
            }
            else
            {
                Console.WriteLine("Serveuse : Merde t'es mineur, tiens prend un verre et rentre chez toi... Vous avez passé la nuit ivre mort à essayer de rentrer chez vous en vain, grâce à vos beuglements aucun mort vivant n'a osé vous attaquer mais vous avez fini par trébucher sur votre jambe et vous noyer dans une flaque d'eau... la vie n'épargne plus les simples d'esprits");
                HandleAge(age, mode);
            }
            return;
        }

        if (ageNum >= 120)
        {
            if (mode == "table")
            {
                Console.WriteLine("Serveuse : Un mort vivant ? SORTEZ !!! Vous vous êtes fais tuer, les gens dans ce bar ont vraiment l'air à cran...");
                HandleAge(age, mode);
            }
            else
            {
                Console.WriteLine("Serveuse : Les morts vivants aussi devrez pouvoir se prélasser... MAIS PAS CE SOIR !!! Vous vous êtes fais tuer, les gens dans ce bar ont vraiment l'air à cran...");
                HandleAge(age, mode);
            }
            return;
        }

        if (ageNum < 27)
        {
            Console.WriteLine(mode == "table"
                ? "Serveuse : Bienvenue, votre nom ?"
                : "Serveuse : Aussi jeune et déjà des problèmes, t'es au bon endroit mon pote, ton nom ?");
            money -= 20;
            Console.WriteLine($"Vous payez 20 crédits pour la table. Il vous reste {money} crédits.");
        }
        else
        {
            Console.WriteLine(mode == "table"
                ? "Serveuse : Et vous venez encore ici à votre grand âge... pffff, et votre prénom ?"
                : "Serveuse : Et ca se met encore des mines à votre grand âge... je compatis, votre prénom ?");
            money -= 20;
            Console.WriteLine($"Vous payez 20 crédits pour la table. Il vous reste {money} crédits.");
        }

        name = Console.ReadLine();

        if (name == "Romain") money = 200;

        HandleName(name, ageNum, mode);
    }

    static void HandleName(string name, int ageNum, string mode)
    {
        switch (name)
        {
            case "Romain":
                Console.WriteLine(ageNum < 27
                    ? "Serveuse : Notre fidèle VIP, BIENVENUE (je l'imaginais plus âgé)"
                    : "Serveuse : Notre fidèle VIP, Bienvenue");
                HandleDrinkChoice(name, ageNum, mode);
                break;

            case "Paul":
                HandlePaulChoice(name, ageNum, mode);
                break;

            default:
                Console.WriteLine("Serveuse : Bienvenue " + name);
                HandleDrinkChoice(name, ageNum, mode);
                break;
        }
    }

    static void HandlePaulChoice(string name, int ageNum, string mode)
    {
        Console.WriteLine("La serveuse appelle le gérant du bar qui se met à une fenêtre et vous tend un papier :Tom t'aime secrètement et souhaite partager sa vie avec toi... 1. Partager sa vie avec Tom   2. Rejeter Tom   3. Faire miroiter un amour réciproque");
        choix = Console.ReadLine();

        int choixNum = int.Parse(choix);

        if (choixNum == 1)
        {
            Console.WriteLine("Tom et toi partez vers l'Ouest sous un soleil couchant vers une probable meilleure vie, nulle ne sait ce qu'il est arrivé de vous et de votre amour interdit...");
        }
        else if (choixNum == 2)
        {
            Console.WriteLine("Tom, fou de chagrin met fin à ses jours, vous ne pouvez vous délecter de l'un des breuvages du Royal Zgueg avant de vous être débarasser du corps ce qui vous épuise");
        }
        else if (choixNum == 3)
        {
            Console.WriteLine("Vous faites mine d'accepter les avances mais retardez votre départ, vous obtenez un traitement de faveur au Royal Zgueg, vous avez accès à l'entièreté de la carte à moitié prix");
            halfPrice = true;
            VIP = true;
            HandleDrinkChoice(name, ageNum, mode);
        }
        else
        {
            Console.WriteLine("Vous jouez la carte de la folie et poussez des cris de goule, Tom n'y est pas insensible, il sort une hache... Votre dépouille est servie comme plat du jour, la prochaine fois on choisit parmis les choix proposés et on ne s'improvise pas casseur de jeu :)");
            Main();
        }
    }

    static void HandleDrinkChoice(string name, int ageNum, string mode)
    {
        Console.WriteLine($"Vous avez actuellement {money} crédits.");
        if (!VIP)
        {
            Console.WriteLine("Serveuse : Que souhaitez-vous boire ? 1. Bière (5 crédits)    2. Royal Zgueg (20 crédits)   3. Jus de Norbert (10 crédits)   4. Bière (5 crédits)   5. Eau   6. Soupe du chef (30 crédits)   7. Bière (5 crédits)   8. Vin (20 crédits)");
        }
        else if (VIP)
        {
            Console.WriteLine("Serveuse : Que souhaitez-vous boire ? 1. Bière (2 crédits)    2. Royal Zgueg (10 crédits)   3. Jus de Norbert (5 crédits)   4. Bière (2 crédits)   5. Eau   6. Soupe du chef (15 crédits)   7. Bière (2 crédits)   8. Vin (10 crédits)");
        }

        choix = Console.ReadLine();
        int choixNum = int.Parse(choix);

        int price = choixNum switch
        {
            1 => 5,
            2 => 20,
            3 => 10,
            4 => 10,
            5 => 0,
            6 => 30,
            7 => 10,
            8 => 20,
            _ => -1
        };

        if (price == -1)
        {
            Console.WriteLine("Choix invalide");
            HandleDrinkChoice(name, ageNum, mode);
        }

        if (halfPrice) price /= 2;

        if (money < price)
        {
            if (upset)
            {
                Console.WriteLine($"Serveuse : Bon allez tu dégages !! Vous avez été mis à la porte, la serveuse en a profité pour faire gagner un fût de Royal Zgueg à quiconque ramènera votre dépouille... c'est bien tous ce que vous valez avec votre manie à jouer avec les nerfs des gens");
                angry = false;
                upset = false;
                HandleDrinkChoice(name, ageNum, mode);
            }
            else if (angry)
            {
                Console.WriteLine($"Serveuse : Je t'ai déja dit que t'avais pas assez de crédits ({money}) pour ça tu fais exprès...");
                upset = true;
                HandleDrinkChoice(name, ageNum, mode);
            }
            else
            {
                Console.WriteLine($"Serveuse : Vous n'avez pas assez de crédits ({money}) pour payer {price}.");
                angry = true;
                HandleDrinkChoice(name, ageNum, mode);
            }
        }

        money -= price;

        switch (choixNum)
        {
            case 1:
                Console.WriteLine(ageNum < 27 ? "Vous avez choisi une bière, un classique que vous vous empresserez de commander à nouveau jusqu'à ne plus tenir debout" : "Vous avez choisi une bière, vous adoreriez pouvoir vous permettre d'en prendre une autre mais... ON SE CALME, vu votre âge vous êtes complètement fauché");
                break;
            case 2:
                Console.WriteLine("Vous avez choisi le Royal Zgueg, un breuvage qui vous fera perdre la raison et vous fera danser toute la nuit, laissez vous transporter dans un univers onirique fait des rêves des précédents consommateurs et des hallucinations les plus étranges");
                break;
            case 3:
                Console.WriteLine("Vous avez choisi le jus de Norbert, la barman vous indique un étrange stand où se trouve un gnome vendant une limonade aux arômes subtiles");
                HandleLimeChoice(name, ageNum, mode);
                break;
            case 4:
                Console.WriteLine(ageNum < 27 ? "Vous avez choisi une bière, un classique que vous vous empresserez de commander à nouveau jusqu'à ne plus tenir debout" : "Vous avez choisi une bière, vous adoreriez pouvoir vous permettre d'en prendre une autre mais... ON SE CALME, vu votre âge vous êtes complètement fauché");
                break;
            case 5:
                Console.WriteLine("Vous avez choisi l'eau, tout le monde dans le bar vous dévisage comme si vous étiez le problème, votre verre arrive et pétille la serveuse vous explique que c'est dû à des restes de liquide vaisselle. Après avoir longuement hésité vous avez fini par boire l'eau, ce qui devait être une simple blague a tourné à l'homicide involontaire, pas de place pour les radins ici...");
                HandleDrinkChoice(name, ageNum, mode);
                break;
            case 6:
                Console.WriteLine("Vous avez choisi la soupe du chef, un met onéreux mais quasi vital dans cet établissement, ça doit bien être la seule manière de se déshiniber tout en gardant un souvenir d'une soirée stable");
                break;
            case 7:
                Console.WriteLine(ageNum < 27 ? "Vous avez choisi une bière, un classique que vous vous empresserez de commander à nouveau jusqu'à ne plus tenir debout" : "Vous avez choisi une bière, vous adoreriez pouvoir vous permettre d'en prendre une autre mais... ON SE CALME, vu votre âge vous êtes complètement fauché");
                break;
            case 8:
                Console.WriteLine("Vous avez choisi le vin... savez-vous seulement où vous êtes ? La barman vous ramène le sourire au lèvre une sélection des bouteilles de la cave parmis lesquelles se trouve : une cuvée canard WC, du liquide vaisselle, du sans-plomb de 98 une excellente année");
                break;
        }

        Console.WriteLine($"Il vous reste {money} crédits.");
    }

    static void HandleLimeChoice(string name, int ageNum, string mode)
    {
        Console.WriteLine("Le gnome d'un geste de la main vous indique sa limonade...   1. Tester la limonade   2. Rejeter l'offre");
        choix = Console.ReadLine();
        int choixNum = int.Parse(choix);
        if (choixNum == 1)
        {
            Console.WriteLine("Vous goûtez à la limonade et vous sentez immédiatement transporté dans un monde où vous voyez les sons et entendez les couleurs... Vous vous réveillez dénudé et enchainé, vous allez passer le reste de votre vie à servir Norbert dans sa quête de perfection de son breuvage en tant que testeur, c'est surement là où la vie voulez vous mener...");
            HandleDrinkChoice(name, ageNum, mode);
        }
        else if (choixNum == 2)
        {
            Console.WriteLine("Vous déclinez l'offre ce qui semble délier la langue du gnome   Norbert : ah mon ami, vous souhaitiez dont me rencontrer pour affaires je suppose ?");
            HandleNorbertChoice(name, ageNum, mode);
        }
        else
        {
            Console.WriteLine("Choix invalide");
            HandleLimeChoice(name, ageNum, mode);
        }
    }

    static void HandleNorbertChoice(string name, int ageNum, string mode)
    {
        Console.WriteLine("Norbert : D'abord rendez moi un service, retournez au bar et demandez la soupe du chef en la mettant sur mon compte !   1. Retourner au bar   2. Refuser le marché");
        choix = Console.ReadLine();
        int choixNum = int.Parse(choix);
        if (choixNum == 1)
        {
            norbertRequest = true;
            HandleDrinkChoice(name, ageNum, mode);
        }
        else if (choixNum == 2)
        {
            Console.WriteLine("Norbert l'air déçu s'en va en laissant son stand derrière lui, vous décidez de récupérer une flasque de sa limonade sans trop savoir quoi en faire");
            getLime = true;
            HandleDrinkChoice(name, ageNum, mode);
        }
        else
        {
            Console.WriteLine("Choix invalide");
            HandleNorbertChoice(name, ageNum, mode);
        }
    }
}