import java.util.ArrayList;

public class MyBot500 {
    private static final int COEFF_ATTACK = 50;
    private static final int COEFF_DEFENSE = 20;
    private static final int COEFF_POPULATION = 10;
    private static final int COEFF_NEGATIVE = -1;
    private static final Direction[] bigDirection = {Direction.WEST, Direction.SOUTH, Direction.EAST, Direction.NORTH};
    private static final Direction[] smallDirection = {Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST};

    public static void main(String[] args) throws java.io.IOException {
        InitPackage iPackage = Networking.getInit();
        int myID = iPackage.myID;
        GameMap gameMap = iPackage.map;

        Networking.sendInit("MyBot500");

        while (true) {
            ArrayList<Move> moves = new ArrayList<Move>();

            gameMap = Networking.getFrame();

            for (int y = 0; y < gameMap.height; y++) {
                for (int x = 0; x < gameMap.width; x++) {
                    Site site = gameMap.getSite(new Location(x, y));
                    if (site.owner == myID) {
                        Direction dir;
                        if (site.strength < 5) {
                            dir = Direction.STILL;
                        } else {
                            dir = getDirection(gameMap, myID, site, new Location(x, y));
                        }
                        moves.add(new Move(new Location(x, y), dir));
                    }
                }
            }
            Networking.sendFrame(moves);
        }
    }

    private static Direction getDirection(GameMap gameMap, int myID, Site site, Location location) {
        Direction direction = Direction.STILL;
        Integer maxGain = getPoints(gameMap, myID, site, location, Direction.STILL);
        Integer currGain = -100000;
        Direction[] directionArray;
        if (site.strength > 150) {
            directionArray = bigDirection;
        } else {
            directionArray = smallDirection;
        }
        for (Direction currDirection : directionArray) {
            currGain = getPoints(gameMap, myID, site, location, currDirection);
            if (maxGain < currGain) {
                direction = currDirection;
                maxGain = currGain;
            }
        }

        return direction;
    }

    private static Integer getPoints(GameMap gameMap, int myID, Site site, Location location, Direction direction) {
        Site opposite = gameMap.getSite(location, direction);
        Integer strength = 0;
        Integer production = 0;
        Integer population = 0;
        Integer overStrength = 0;
        if (!Direction.STILL.equals(direction)) {
            if (myID == opposite.owner) {
                strength = (opposite.strength + site.strength) * COEFF_DEFENSE;
                if (opposite.strength == 0 || opposite.strength >= 240) {
                    strength *= COEFF_NEGATIVE;
                }
            } else {
                strength = (opposite.strength * COEFF_NEGATIVE + site.strength) * COEFF_ATTACK;
                if (strength > 0) {
                    population = COEFF_POPULATION;
                    production = (strength + opposite.production) * COEFF_DEFENSE;
                }
            }
        } else {
            production = site.production * COEFF_DEFENSE;
        }
        if (site.strength > 240) {
            if (myID != opposite.owner) {
                overStrength = opposite.strength * COEFF_ATTACK;
            }
        } else {
            if (strength > 255) {
                overStrength = opposite.strength * COEFF_ATTACK * COEFF_NEGATIVE;
            }
        }
        return strength + production + population + overStrength;
    }
}
