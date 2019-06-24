class AnnotationEditor {
	constructor(lot, canvas) {
		this.lot = lot;
		this.canvas = canvas;
		// resize canvas to fit screen resolution
		this.canvas.width = this.canvas.clientWidth;
		this.canvas.height = this.canvas.clientHeight;
		// Display constants 
		this.lineWidth = 1;
		this.pointRadius = 8;
		this.pointDisplayRadius = 2;
		// Selecting points
		this.shift = false;
		this.ctrl = false;
		this.selection = [];
		this.clipboard = [];
		// Drawing new annotations
		this.drawMode = 'Select';
		this.drawing = null; // the annotation being drawn
		// Setup event listeners
		this.dragging = null; // if the selection is being dragged
		this.hover = null; // the point being hovered over
		this.canvas.addEventListener('mousemove', (evt) => {
			let x = evt.offsetX / this.canvas.width;
			let y = evt.offsetY / this.canvas.height;
			if (this.dragging && this.selection.length > 0) {
				let dx = x - this.dragging.X;
				let dy = y - this.dragging.Y;
				this.selection.forEach(p => {
					p.X += dx;
					p.Y += dy;
				});
			} else if (this.drawing) {
				this.drawing.Points[1].X = x;
				this.drawing.Points[3].Y = y;
				this.drawing.Points[2].X = x;
				this.drawing.Points[2].Y = y;
			}
			this.hover = null;
			this.canvas.style.cursor = 'default';
			this.lot.Annotations.forEach(annotation => {
				if (annotation.Points.forEach(point => {
					let screen = this.toScreen(point);
					let x = screen.X - evt.offsetX;
					let y = screen.Y - evt.offsetY;
					if (x * x + y * y <= this.pointRadius * this.pointRadius) {
						this.canvas.style.cursor = 'pointer';
						this.hover = point;
						return true;
					}
				}))
					return true;
			});
		});
		this.canvas.addEventListener('mousedown', (evt) => {
			if (evt.button === 0) {
				if (this.drawMode === 'Select') {
					this.lot.Annotations.forEach(annotation => {
						annotation.Points.forEach(point => {
							let screen = this.toScreen(point);
							let x = screen.X - evt.offsetX;
							let y = screen.Y - evt.offsetY;
							if (x * x + y * y <= this.pointRadius * this.pointRadius) {
								if (!this.selected(point)) {
									if (this.shift) {
										this.selection.push(point);
									} else {
										this.selection = [point];
									}
								}
								this.dragging = point;
								return true;
							}
						});
					});
					if (this.dragging)
						return;

				}
				this.drawing = new Annotation(this.drawMode);
				this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
				this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
				this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
				this.drawing.Points.push(new Vector2(evt.offsetX / this.canvas.width, evt.offsetY / this.canvas.height));
				
				this.selection = [];
					
			}
		});
		window.addEventListener('keydown', (evt) => {
			// Shift
			if (evt.keyCode === 16) {
				this.shift = true;
			}
			// Delete
			else if (evt.keyCode === 46) {
				for (let i = 0; i < this.lot.Annotations.length; i++) {

					let annotation = this.lot.Annotations[i];

					if (this.selected(annotation)) {
						this.lot.Annotations.splice(i, 1);
						i--;
					}
				}
			}
			// Ctrl
			else if (evt.keyCode === 17) {
				this.ctrl = true;
			}
			// C
			else if (evt.keyCode === 67) {
				if (this.ctrl) {
					this.clipboard = [];
					this.lot.Annotations.forEach(a => {
						if (this.selected(a)) {
							this.clipboard.push(a);
						}
					});
				}
			}
			// V
			else if (evt.keyCode === 86) {
				if (this.ctrl) {
					this.selection = [];
					this.clipboard.forEach(a => {
						let annotation = {
							ID: -1,
							Points: [],
							Type: a.Type
						};
						a.Points.forEach(p => {
							let dx = 10 / this.canvas.width;
							let dy = 10 / this.canvas.width;
							let point = new Vector2(p.X + dx, p.Y + dy);
							this.selection.push(point);
							annotation.Points.push(point);
						});
						this.lot.Annotations.push(annotation);
					});
				}
			}
		});
		window.addEventListener('keyup', (evt) => {
			if (evt.keyCode === 16) {
				this.shift = false;
			} else if (evt.keyCode === 17) {
				this.ctrl = false;
			}
		});
		window.addEventListener('mouseup', (evt) => {
			if (evt.button === 0) {
				this.dragging = null;
				if (this.drawing !== null) {
					if (this.drawMode === 'Select') {
						this.lot.Annotations.forEach(a => {
							a.Points.forEach(p => {
								if (this.drawing.contains(p)) {
									this.selection.push(p);
								}
							});
						});
					} else {
						this.lot.Annotations.push(this.drawing);
					}
					this.drawing = null;
					this.drawMode = 'Select';
				}
			}
		});
		// Bootstrap animation loop
		this.tick();
	}
	selected(obj) {
		if (obj instanceof Vector2) {
			for (let i = 0; i < this.selection.length; i++) {
				if (this.selection[i] === obj) {
					return true;
				}
			}
		} else {
			let allContained = true;
			for (let j = 0; j < obj.Points.length; j++) {
				if (!this.selected(obj.Points[j])) {
					allContained = false;
					break;
				}
			}
			return allContained;
		}
		return false;
	}
	toScreen(point) {
		return {
			X: this.canvas.width * point.X, Y: this.canvas.height * point.Y
		};
	}
	tick() {
		this.update();
		this.render();
		window.requestAnimationFrame(() => {
			this.tick();
		});
	}
	update() {
		
	}
	render() {
		let self = this;
		let ctx = this.canvas.getContext('2d');
		ctx.strokeStyle = 'black';
		ctx.setLineDash([]);
		ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
		function renderAnnotation(ctx,annotation) {
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
			switch (annotation.Type) {
				case 'Parking': ctx.fillStyle = 'rgba(255,0,0,0.5)'; break;
				case 'Isle': ctx.fillStyle = 'rgba(0,0,255,0.5)'; break;
				case 'Select': ctx.fillStyle = 'rgba(0,0,192,0.25)'; break;
			}
			ctx.fill();
			ctx.lineWidth = self.lineWidth;
			ctx.stroke();

			annotation.Points.forEach((point, i) => {
				let screen = self.toScreen(point);
				ctx.beginPath();
				ctx.arc(screen.X, screen.Y, point === self.hover ? self.pointRadius : self.pointDisplayRadius, 0, Math.PI * 2);
				
				ctx.fillStyle = self.selected(point) ? 'blue' : 'black';
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
	save() {
		$.ajax({
			url: '/ParkingLot/Update',
			method: 'POST',
			data: this.lot
		}).done(data => {

		});
	}
	newSpace() {
		this.drawMode = 'Parking';
	}
	newIsle() {
		this.drawMode = 'Isle';
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