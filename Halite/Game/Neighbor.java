public class Neighbor {
    public Direction direction;
    public boolean enemy;
    public int strength;
    public int production;

    public Neighbor(Direction direction, boolean enemy, int strength, int production) {
        this.direction = direction;
        this.enemy = enemy;
        this.strength = strength;
        this.production = production;
    }
}
