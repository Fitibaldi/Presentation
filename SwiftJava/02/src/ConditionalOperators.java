import java.util.Scanner;

public class ConditionalOperators {

	public static void main(String[] args) {
		Scanner sc = new Scanner(System.in);
		System.out.print("Please enter grade: ");
		byte grade = sc.nextByte();
//		byte grade = 4;

		switch (grade) {
		case 2:
			System.out.println("Bad");
			break;
		case 3:
			System.out.println("Not so bad");
			break;
		case 4:
			System.out.println("Good");
			break;
		case 5:
		case 6:
			System.out.println("Excellent");
			break;
		default:
			System.out.println("No such grade");
			break;
		}
	}

}
