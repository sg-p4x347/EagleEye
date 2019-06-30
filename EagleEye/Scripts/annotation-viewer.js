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
			if (typeof self.prepareAnnotationRender === 'function') {
				self.prepareAnnotationRender(self,ctx,annotation);
			}
			ctx.fill();
			ctx.lineWidth = self.lineWidth;
			ctx.stroke();

			annotation.Points.forEach((point, i) => {
				let screen = self.toScreen(point);
				ctx.beginPath();
				ctx.arc(screen.X, screen.Y, point === self.hover ? self.pointRadius : self.pointDisplayRadius, 0, Math.PI * 2);

				self.preparePointRender(self,ctx, point);
				ctx.fill();
				ctx.stroke();
			});
		}
		this.lot.Annotations.forEach(annotation => {
			renderAnnotation(ctx, annotation);
		});
		if (this.drawMode !== 'Select') {
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
	constructor(type) {
		this.ID = -1;
		this.Type = type;
		this.Points = [];
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
		let area = 0;
		for (let i = 0; i < this.Points.length; i++) {
			area += this.triangleArea(point, this.Points[i], this.Points[(i + 1) % this.Points.length]);
		}
		return Math.abs(area - this.area) <= 0.001;
	}
}