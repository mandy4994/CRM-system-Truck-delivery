# CRM-system-Truck-delivery

## Requirements 1st part of assignment:

In this assignment, we use one business rule: delivery fees consist of a fixed amount of
$1000 per truck, plus insurance charge, and plus a variable amount depending on variable hourly 
rates (as defined in Appendix A and B) and delivery times. The insurance fee is equal to 10 cents 
per kilometre of the travelled distance. As for the variable hourly rates, Appendix A lists the 
hourly rates varying on time, date and location; and Appendix B lists Australian public holidays. 
The CRM system must consider deliveries  that  travel  between week/weekends, multiple time periods 
and/or states/territories. The CRM system will make a number of assumptions in regards to computing 
delivery times and costs, as is listed below. Additionally a complete list of Australian post  
codes  is  provided  in  CSV  format  on CloudDeakin which may assist you when estimating the 
distance between an origin and destination.

### Assumptions

As mentioned, the CRM system is expected to make a number of assumptions when calculating delivery 
times and costs in order to simplify the computation required. These assumptions are:

• A truck driver will always drive at a constant speed. Alternatively the CRM system may consider 
variable speeds (e.g. a truck will not travel the same speed in a suburban area as on a highway).
• A truck driver will always work for a constant amount of hours before stopping to rest. Time 
spent resting is unpaid.
• Deliveries to/from Tasmania will be ferried, a ferry will always be available upon arrival, will 
cost a fixed amount (that the customer gets charged for), and takes a constant amount of time to 
ferry between Tasmania and Victoria.
• When traveling interstate, a truck spends an equally divided amount of time and/or distance in 
each state (e.g. a delivery from Victoria to Western Australia 2,000 KM long consists of 666.67 KM 
travelled in each of Victoria, South Australia, and Western Australia). Alternatively the CRM 
system may consider more accurate representations.


### Front End

You will need to create an ASP .NET website with one web form. You do not need any other web pages 
other than the one with the web form. On this web form (e.g. “Default.aspx”), there should be at 
least the following:

•   Several controls (e.g. Textbox, NumericUpDown) for inputting:
o  First Name
o Last Name
o  Origin delivery address
o  Origin postcode
o  Destination delivery address
o  Destination postcode
o  Billing address o  Phone number o  Delivery date
o  Delivery time (Note: The delivery should arrive at or before this date/time, so you will have to 
calculate the start time and date)
o  Number of trucks to hire
•   Several “Label” controls to indicate the compulsory input fields and error messages.
•   A set of appropriate ASP.NET “Validators” to validate user input from the server side.
•  A CSS component.
•   A reference to the DLL file which generates PDF files.
•   And a “Button” control to submit the data.

### Back End

In the C# file associated with the form (e.g., “Default.aspx.cs”), you should write C# code to 
implement the following actions:

1.     Retrieve the user input and place them into appropriate variables.
2.    Process a CSV file for postcode information by using LINQ or other methods.
3.     Implement the business rules according to different time, days and locations.
4.     Apply the business rules for each truck hiring case including insurance cost.
5.    Derive the amount to charge and its GST.
6.    Generate a unique invoice number.
7.     Generate a PDF file with the invoice number, your business address, customer’s billing 
address, current system time, the invoice items (i.e., how many trucks), promised delivery date and 
time, total cost and GST.

Additionally, in order to demonstrate the correctness of your implementation of the business rule, 
you should include a few sample invoice PDF files which include the following cases:

•   Multiple trucks are hired to deliver goods across two adjacent states.
•   A delivery takes at least 4 days (must include the period between 25/4/2015 and 28/4/2015) to 
complete.

## Requirements 2nd part of assignment:
It	involves	development	of	an	integrated	system	which	retrieves	clients’	orders
from	 Assignment	 1,	 issues	 invoices	 with	 appropriate	 prices, retrieves	 membership	
information,	 generates a rebate	 statement	 and	 stores	 transactional	messages	 accordingly.	
More	concretely,	we	need	an	additional	component	– rebate	processor	integrated	with	the	
two	components	developed	in	assignments	1	and	2.	Your	task	is	to	calculate	the	rebate	price	
according	 to	 the	membership	 classes	 (Gold,	 Silver	 or	 Regular	as	 defined	 in	 Assignment	 2).	
The	rebate	value	should	be	derived	as	following:	
• Regular	members	will	receive	$200	credit	for	each	booking.
• Silver	members	will	receive	15%	discount	for	every	$5,000	spent	(excluding	GST).
• Gold	members	will	receive	$300	credit	for	every	booking	and	20%	discount	for	every	
$5,000	spent	(excluding	GST).	
Note:	 Discount	 price	 should	 be	 awarded	 in	 the	 tax	 invoice	 during	 the	 current	 transaction;	
rebate	 credit	 should	 be	 awarded	 after	 issuing	 the	 tax	 invoice,	 and	 the	 amount	 will	 be	
redeemed	in	the	next	transaction.	Moreover,	do	not	attempt to improve	your	calculations	of	
delivery	distance	and	price,	but	you	should	use	the	numbers	produced from	your	assignment	
1	solution.	
Hence,	your	rebate	processor	should	store	the	information	of	previous	transactions.	

Your	 first	 task	 is	 to	 link	 the	 first	 two	 assignment	 solutions	 by	 adding	 a	 textbox	 for	
membership	 ID	 (mapping	 to	 “MembershipID”	 in	 the	 membership	 database	 table)	 in	 the	
truck	booking	web	form.	More	specifically,	if	the	supplied	ID	matches	an	existing	member	in	
the	 database,	 then	 this	member’s	 first	 name	 and	 last	 name	 should	 be	 retrieved	 from	 the	
database	and	displayed	in	the	booking	form;	if	the	supplied	ID	does	not	match	any	existing	
member	in	the	database,	then	the	system	should	insert	this	customer	as	a	regular	member	
to	the	membership	table.	

The	 next	 step	 is	 to	 implement	 the	 rebate	 processor	 which	 should	 store	 every	 booking	
record,	 calculate	 and	 apply	 rebate	 prices.	 Each	 booking	 should	 be	 referred	 by	 the	 unique	
invoice	number generated	in	assignment	1.	

Then, port	 your	 modified	 booking	 processor	 (as	 in	 assignment	 1),	 rebate	 processor,	 and	
database	accessing	APIs	 (as	in	assignment	 2)	 on at	least	 three	 separate	 threads	and	apply	
appropriate	 multi-threading	 controls such	 as semaphores/mutex/wait;	 alternatively,	 you	
may	 use	 async/await.	 This	 will	 set	 you	 up	 for	 the	 final	 step	 where	 you	 will	 implement	 a	
message	queue	for	the	rebate	processor.

Finally, you	need	to	set	up	appropriate	ActiveMQ	message	queues	which	link	the	booking	
processor	and	rebate	processor	by	sending	and	receiving	messages.	One	challenge	you	will	
have	to	consider	is	how	you	will	convert the	truck	order form’s data	to	a	message	so	that	it	
can	 be	 put	 into	 a	 message	 queue,	 at	 which	 point	 your	 rebate	 processor	 must	 be	 able	 to	
parse	each	message. Another	challenge	to	consider	will	be	how	you	can convert	your	rebate	
processor’s	results	to	a	message	so	they	can	be	sent	back.

Note:	 You	 should	 submit	 this	 assignment	 as	 an	 integrated	 system	 which	 includes	 the	
previous	two	assignment	solutions	with	necessary	modifications.	

In	order	to	demonstrate	the	correctness	of	your	implementation	of	the	entire	system,	you	
should	include	a	few	sample	invoice	PDF	files	and	sample	rebate	statements	(in	any	readable	
format)	which	include	the	following	cases:
• An	existing	gold	member	hires	five trucks	to	deliver	goods	across	two	adjacent	states,	
and	then	the same	customer	makes a second	booking	hiring three trucks	to	deliver	
goods	during	a	weekend.	
• An	existing	silver	member	hires	two	trucks for	more	than	4 days	to	deliver	goods.	
• A	 new	 customer	 hires	 one	 truck	 to	 deliver	 goods	 during	 2015 Christmas holidays,	
and	 then	 the same	 customer	makes	a second	 booking	 to	 hire	 one	 truck	 to	 deliver	
goods	during	a	weekend.	
