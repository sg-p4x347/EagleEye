class AnnotationViewer {
	constructor(lot, canvas, prepareAnnotationRender, preparePointRender) {
		this.lot = lot;
		this.canvas = canvas;
		this.prepareAnnotationRender = prepareAnnotationRender;
		this.preparePointRender = preparePointRender;
		// resize canvas to fit screen resolution
		this.canvas.width = this.canvas.clientWidth;
		this.canvas.height = this.canvas.clientHeight;
		// Display constants 
		this.lineWidth = 1;
		this.pointRadius = 8;
		this.pointDisplayRadius = 2;
		
	}
	get start() {
		return this.m_start;
	}
	set start(value) {
		this.m_start = value;
		if (this.goal)
			this.setPath(this.aStar(this.start, this.goal.midpoints));
	}
	get goal() {
		return this.m_goal;
	}
	set goal(value) {
		this.m_goal = value;
		if (this.start)
			this.setPath(this.aStar(this.start, this.goal.midpoints));
	}
	setPath(path) {
		this.path = path;
	}
	toScreen(point) {
		return {
			X: this.canvas.width * point.X, Y: this.canvas.height * point.Y
		};
	}
	tick() {
		this.render();
		window.requestAnimationFrame(() => {
			this.tick();
		});
	}
	render() {
		let self = this;
		let ctx = this.canvas.getContext('2d');
		ctx.strokeStyle = 'black';
		ctx.setLineDash([]);
		ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
		function renderAnnotation(ctx, annotation) {
			
			if (self.prepareAnnotationRender(self, ctx, annotation)) {
				ctx.beginPath();
				annotation.Points.forEach((point, i) => {
					let screen = self.toScreen(point);
					if (i === 0) {
						ctx.moveTo(screen.X, screen.Y);
					} else {
						ctx.lineTo(screen.X, screen.Y);
					}
				});
				ctx.closePath();
				ctx.fill();
				ctx.stroke();
				
			}
			
			
			annotation.Points.forEach((point, i) => {
				if (self.preparePointRender(self, ctx, annotation,point)) {
					let screen = self.toScreen(point);
					ctx.beginPath();
					ctx.arc(screen.X, screen.Y, point === self.hover ? self.pointRadius : self.pointDisplayRadius, 0, Math.PI * 2);

					ctx.fill();
					ctx.stroke();
				}
			});
		
		}
		this.lot.Annotations.forEach(annotation => {
			renderAnnotation(ctx, annotation);
		});
		if (this.drawMode !== undefined && this.drawMode !== 'Select') {
			ctx.fillStyle = 'rgba(255,255,255,0.5)';
			ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
			if (this.drawing) {
				renderAnnotation(ctx, this.drawing);
			}
		} else if (this.drawing) {
			ctx.setLineDash([5, 5]);
			ctx.strokeStyle = 'blue';
			renderAnnotation(ctx, this.drawing);
		}

		if (this.path) {
			ctx.beginPath();
			ctx.strokeStyle = 'blue';
			ctx.lineWidth = '8';
			ctx.lineCap = 'round';
			ctx.lineJoin = 'round';
			this.path.forEach((point, i) => {
				let screen = self.toScreen(point);
				if (i === 0) {
					ctx.moveTo(screen.X, screen.Y);
				} else {
					ctx.lineTo(screen.X, screen.Y);
				}
			});
			ctx.stroke();
		}

	}
	generateNodes() {
		let nodes = [];
		this.lot.Annotations.where(an => an.Type === "Isle").forEach(an => {
			let midpoints = an.midpoints;
			for (let i = 0; i < 2; i++) {
				let start = midpoints[i];
				let end = midpoints[i + 2];
				let segment = end.subtract(start);
				let step = segment.length / Math.ceil(segment.length / AnnotationViewer.nodeDistance);
				for (let t = 0; t <= segment.length; t += step) {
					nodes.push(start.add(segment.normalized().scale(t)));
				}
			}
		});
		return nodes;
	}
	
	aStarReconstructPath(cameFrom, current) {
		let totalPath = [current];
		while (cameFrom.get(current)) {
			current = cameFrom.get(current);
			totalPath.push(current);
		}
		return totalPath;
	}
	aStarHeuristicCostEstimate(a, b) {
		if (a === b) {
			return 0;
		} else {
			return a.subtract(b).length;
		}
	}
	aStarNeighbors(nodes,a) {
		return nodes.where(n => {
			return n !== a && !this.lot.Annotations.any(an => an.Type === 'Parking' && an.intersects(a,n));
		});
	}
	static get nodeDistance() {
		return 0.05;
	}

	aStar(start, goal) {
		let nodes = this.generateNodes();
		nodes.push(start);
		goal.forEach(n => nodes.push(n));
		// The set of nodes already evaluated
		let closedSet = [];

		// The set of currently discovered nodes that are not evaluated yet.
		// Initially, only the start node is known.
		let openSet = [start];

		// For each node, which node it can most efficiently be reached from.
		// If a node can be reached from many nodes, cameFrom will eventually contain the
		// most efficient previous step.
		let cameFrom = new HashMap();

		// For each node, the cost of getting from the start node to that node.
		let gScore = new HashMap();
		nodes.forEach(n => {
			gScore.set(n, Infinity);
		});
		
		// The cost of going from start to start is zero.
		gScore.set(start, 0);

		// For each node, the total cost of getting from the start node to the goal
		// by passing by that node. That value is partly known, partly heuristic.
		let fScore = new HashMap();
		nodes.forEach(n => {
			fScore.set(n, Infinity);
		});
		

		// For the first node, that value is completely heuristic.
		fScore[start] = this.aStarHeuristicCostEstimate(start, goal[0]);

		let current = start;
		while (openSet.length > 0) {
			let min = { node: null, fScore: Infinity };
			for (let i = 0; i < openSet.length; i++) {
				let value = fScore.get(openSet[i]);
				if (value <= min.fScore) {
					min = { node: openSet[i], fScore: value };
				}
			}
			current = min.node;

			if (goal.contains(current))
				return this.aStarReconstructPath(cameFrom, current);

			openSet.remove(current);

			closedSet.push(current);

			this.aStarNeighbors(nodes, current).forEach(neighbor => {

				if (closedSet.contains(neighbor))
					return; // Ignore the neighbor which is already evaluated.

				// The distance from start to a neighbor
				let tentativeGScore = gScore.get(current) + current.subtract(neighbor).length;


				if (!openSet.contains(neighbor)) {  // Discover a new node
					openSet.push(neighbor);
				}
				else if (tentativeGScore >= gScore.get(neighbor)) {
					return;
				}

				// This path is the best until now. Record it!
				cameFrom.set(neighbor, current);

				gScore.set(neighbor,tentativeGScore);

				fScore.set(neighbor, tentativeGScore + goal.minValue(g => this.aStarHeuristicCostEstimate(neighbor, g)));
			});
			
		}
		// A valid path was not found, return a path containing just the start node
		return this.aStarReconstructPath(cameFrom, current);
		
	}
}
class HashMap {
	constructor() {
		this.map = {};
	}
	set(key, value) {
		this.map[JSON.stringify(key)] = value;
	}
	get(key) {
		return this.map[JSON.stringify(key)];
	}
	remove(key) {
		delete this.map[JSON.stringify(key)];
	}
}
class Vector2 {
	constructor(x, y) {
		this.X = x;
		this.Y = y;
	}
	add(b) {
		return new Vector2(this.X + b.X, this.Y + b.Y);
	}
	scale(scalar) {
		return new Vector2(this.X * scalar, this.Y * scalar);
	}
	subtract(b) {
		return new Vector2(this.X - b.X, this.Y - b.Y);
	}
	dot(b) {
		return b.X * this.X + b.Y * this.Y;
	}
	get normal() {
		return new Vector2(-this.Y, this.X);
	}
	normalized() {
		let length = this.length;
		return new Vector2(this.X / length, this.Y / length);
	}
	get length() {
		return Math.sqrt(this.X * this.X + this.Y * this.Y);
	}
	
}
class Annotation {
	constructor(props) {
		this.ID = -1;
		this.Points = [];
		for (let prop in props) {
			this[prop] = props[prop];
		}
	}
	get area() {
		return this.triangleArea(this.Points[0], this.Points[1], this.Points[2])
			+ this.triangleArea(this.Points[0], this.Points[2], this.Points[3]);
	}
	triangleArea(a, b, c) {
		let ab = b.subtract(a);
		return ab.length * 0.5 * Math.abs(ab.normal.normalized().dot(c.subtract(a)));
	}
	contains(point) {
		return SAT(this.Points, [point]);
	}
	intersects(a, b) {
		return SAT(this.Points, [a, b]);
	}
	get midpoints() {
		let midpoints = [];
		for (let i = 0; i < 4; i++) {
			let a = this.Points[i];
			let b = this.Points[(i + 1) % 4];
			midpoints.push(a.add(b).scale(0.5));
		}
		return midpoints;
	}
}
function SAT(A, B) {
	let axes = [];
	if (A.length > 1)
		A.forEach((p, i) => {
			axes.push(p.subtract(A[(i + 1) % A.length]).normal.normalized());
		});
	if (B.length > 1)
		B.forEach((p, i) => {
			axes.push(p.subtract(B[(i + 1) % B.length]).normal.normalized());
		});
	let intersection = true;
	axes.forEach(axis => {
		let projA = { min: Infinity, max: -Infinity };
		let projB = { min: Infinity, max: -Infinity };
		A.forEach(p => {
			let proj = p.dot(axis);
			if (proj < projA.min)
				projA.min = proj;
			if (proj > projA.max)
				projA.max = proj;
		});
		B.forEach(p => {
			let proj = p.dot(axis);
			if (proj < projB.min)
				projB.min = proj;
			if (proj > projB.max)
				projB.max = proj;
		});
		if (!(projA.min < projB.max && projB.min < projA.max)) {
			intersection = false;
			return true;
		}
	});
	return intersection;
}