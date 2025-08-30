public static class MoneyManager {

    private static int money = 0;

    public static int Money => money;

    public static void AlterMoney(int amount)
    {
        if(amount >= 0)
        {
            AddMoney(amount);
        }
        else
        {
            RemoveMoney(-amount);
        }
    }

    public static void AddMoney(int amount) {
        money += amount;
    }

    public static void RemoveMoney(int amount) {
        money -= amount;
        if (money <= 0) {
            money = 0;
            // Trigger Bankrupt Event
            GameManager.Instance.Bankrupt();
        }
    }
    
    public static void ResetMoney() {
        money = 0;
    }
}